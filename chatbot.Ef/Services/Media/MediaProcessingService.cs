using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services.Media;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using System.Text.Json;
using chatbot.Core.Models;
using Microsoft.Extensions.Logging;
using chatbot.Core.DTOs;

namespace chatbot.Ef.Services.Media
{
    public class MediaProcessingService : IMediaProcessingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IFileProcessorFactory _processorFactory;
        private readonly IChatRealtimeService _realtimeService;
        private readonly ILogger<MediaProcessingService> _logger;

        public MediaProcessingService(IUnitOfWork unitOfWork, IStorageService storageService, 
            IFileProcessorFactory processorFactory, IChatRealtimeService realtimeService, ILogger<MediaProcessingService> logger)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _processorFactory = processorFactory;
            _realtimeService = realtimeService;
            _logger = logger;
        }

        public async Task ProcessAttachmentAsync(Guid storedFileId, CancellationToken cancellationToken = default)
        {
            var file = await _unitOfWork.StoredFiles
                    .GetForProcessingAsync(storedFileId);

            if (file is null || file.IsDeleted)
            {
                return;
            }

            // Hangfire retry/idempotency
            if (file.ProcessingStatus == FileProcessingStatus.Completed)
            {
                return;
            }

            if (file.Category is FileCategory.Document or FileCategory.Other)
            {
                file.ProcessingStatus = FileProcessingStatus.Completed;

                file.ProcessingCompletedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                return;
            }

            file.ProcessingStatus = FileProcessingStatus.Processing;

            file.ProcessingStartedAt = DateTime.UtcNow;

            file.ProcessingError = null;

            await _unitOfWork.SaveChangesAsync();

            try
            {
                await using var stream = await _storageService.DownloadAsync(file.StorageKey, cancellationToken);

                var processor = _processorFactory.GetProcessor(file.Category);

                var result = await processor.ProcessAsync(stream, file.ContentType, cancellationToken);

                file.Width = result.Width;

                file.Height = result.Height;

                file.DurationSeconds = result.DurationSeconds;

                // Thumbnail handled next
                if (result.ThumbnailStream != null)
                {
                    await CreateThumbnailAsync(file, result, cancellationToken);
                }

                file.ProcessingStatus = FileProcessingStatus.Completed;

                file.ProcessingCompletedAt = DateTime.UtcNow;

                file.ProcessingError = null;

                await _unitOfWork.SaveChangesAsync();

                var attachment = file.MessageAttachments.FirstOrDefault();

                if (attachment != null)
                {
                    await _realtimeService.MediaProcessedAsync(attachment.Message.ConversationId, attachment.MessageId, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                file.ProcessingStatus =FileProcessingStatus.Failed;

                file.ProcessingError =ex.Message;

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception saveException)
                {
                    _logger.LogError(saveException, "Failed to save processing failure for file {FileId}.", file.Id);
                }

                throw;
                
            }
        }

        public async Task ProcessVoiceNoteAsync(Guid voiceNoteId, CancellationToken cancellationToken = default)
        {
            var voiceNote =await _unitOfWork.VoiceNotes.GetForProcessingAsync(voiceNoteId);

            if (voiceNote == null)
                return;

            var file =voiceNote.File;

            if (file.ProcessingStatus == FileProcessingStatus.Completed)
            {
                return;
            }

            file.ProcessingStatus =FileProcessingStatus.Processing;

            file.ProcessingStartedAt = DateTime.UtcNow;

            file.ProcessingError = null;

            await _unitOfWork.SaveChangesAsync();

            try
            {
                await using var stream = await _storageService.DownloadAsync( file.StorageKey,cancellationToken);

                var processor = _processorFactory.GetProcessor( FileCategory.Audio);

                var result =await processor.ProcessAsync(stream,file.ContentType,cancellationToken);

                var duration = result.DurationSeconds ?? 0;

                file.DurationSeconds = duration;

                voiceNote.DurationSeconds = duration;

                voiceNote.Waveform = JsonSerializer.Serialize(
                        result.Waveform ?? new List<double>());

                file.DurationSeconds =result.DurationSeconds;

                file.ProcessingStatus =FileProcessingStatus.Completed;

                file.ProcessingCompletedAt =DateTime.UtcNow;

                file.ProcessingError = null;

                await _unitOfWork.SaveChangesAsync();

                await _realtimeService.VoiceNoteProcessedAsync( voiceNote.Message.ConversationId, voiceNote.MessageId,cancellationToken);
            }
            catch (Exception ex)
            {
                file.ProcessingStatus =FileProcessingStatus.Failed;

                file.ProcessingError = ex.Message;


                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception saveException)
                {
                    _logger.LogError( saveException, "Failed to save voice processing failure for {VoiceNoteId}.",voiceNoteId);
                }

                throw;
            }
        }

        private async Task CreateThumbnailAsync(StoredFile originalFile,MediaProcessingResultDto result,CancellationToken cancellationToken)
        {
            if (result.ThumbnailStream == null)
                return;

            if (originalFile.ThumbnailFileId.HasValue)
            {
                // Important for Hangfire retries.
                return;
            }

            if (result.ThumbnailStream.CanSeek)
            {
                result.ThumbnailStream.Position = 0;
            }

            var fileName =
                $"{Guid.NewGuid():N}.jpg";

            var upload =await _storageService.UploadAsync(
                    result.ThumbnailStream,fileName,result.ThumbnailContentType
                        ?? "image/jpeg",$"messages/thumbnails/{DateTime.UtcNow:yyyy/MM}",
                    cancellationToken);

            var thumbnail =new StoredFile
                {
                    Id = Guid.NewGuid(),

                    OriginalFileName =fileName,

                    StoredFileName =fileName,

                    Extension =".jpg",

                    ContentType =result.ThumbnailContentType ?? "image/jpeg",

                    FileSize =result.ThumbnailStream.CanSeek
                            ? result.ThumbnailStream.Length : 0,

                    Category =FileCategory.Image,

                    Provider =upload.Provider,

                    StorageKey =upload.StorageKey,

                    CreatedAt =DateTime.UtcNow,

                    ProcessingStatus = FileProcessingStatus.Completed,

                    ProcessingCompletedAt =DateTime.UtcNow
                };

            await _unitOfWork.StoredFiles.AddAsync(thumbnail );

            originalFile.ThumbnailFileId = thumbnail.Id;

            originalFile.ThumbnailFile = thumbnail;
        }
    }
}