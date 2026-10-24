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

            if (file is null || file.IsDeleted || file.IsPhysicallyDeleted)
            {
                return;
            }

            // Hangfire retry/idempotency
            if (file.ProcessingStatus == FileProcessingStatus.Completed)
            {
                return;
            }

            // No processing required
            if (file.MediaType is MediaType.Document or MediaType.Other)
            {
                file.ProcessingStatus = FileProcessingStatus.NotRequired;

                file.ProcessingCompletedAt = DateTime.UtcNow;

                file.ProcessingError = null;

                await _unitOfWork.SaveChangesAsync();

                return;
            }

            file.ProcessingStatus = FileProcessingStatus.Processing;

            file.ProcessingStartedAt = DateTime.UtcNow;

            file.ProcessingCompletedAt = null;

            file.ProcessingError = null;

            await _unitOfWork.SaveChangesAsync();

            try
            {
                await using var stream = await _storageService.DownloadAsync(file.StorageKey,file.Provider,cancellationToken)
                ?? throw new FileNotFoundException($"Stored file '{file.Id}' was not found.");

                var processor = _processorFactory.GetProcessor(file.MediaType);

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
                await MarkAsFailedAsync(file, ex);

                throw;
            }
        }

        public async Task ProcessVoiceNoteAsync(Guid voiceNoteId, CancellationToken cancellationToken = default)
        {
            var voiceNote =await _unitOfWork.VoiceNotes.GetForProcessingAsync(voiceNoteId);

            if (voiceNote == null)
                return;

            var file =voiceNote.File;
            if (file.IsDeleted || file.IsPhysicallyDeleted)
            {
                return;
            }

            if (file.ProcessingStatus == FileProcessingStatus.Completed)
            {
                return;
            }
            if (file.MediaType != MediaType.Audio)
            {
                throw new InvalidOperationException("Voice note file must be an audio file.");
            }

            file.ProcessingStatus =FileProcessingStatus.Processing;

            file.ProcessingStartedAt = DateTime.UtcNow;

            file.ProcessingCompletedAt = null;

            file.ProcessingError = null;

            await _unitOfWork.SaveChangesAsync();

            try
            {
                await using var stream = await _storageService.DownloadAsync(file.StorageKey, file.Provider, cancellationToken )
                ?? throw new FileNotFoundException($"Voice file '{file.Id}' was not found.");

                var processor = _processorFactory.GetProcessor(MediaType.Audio);


                var result =await processor.ProcessAsync(stream,file.ContentType,cancellationToken);

                var duration = result.DurationSeconds ?? 0;
                // Exact metadata
                file.DurationSeconds = duration;

                // VoiceNote entity currently uses int
                voiceNote.DurationSeconds = (int)Math.Round(duration, MidpointRounding.AwayFromZero);

                voiceNote.Waveform = JsonSerializer.Serialize(result.Waveform ?? new List<double>());


                file.ProcessingStatus =FileProcessingStatus.Completed;

                file.ProcessingCompletedAt =DateTime.UtcNow;

                file.ProcessingError = null;

                await _unitOfWork.SaveChangesAsync();

                await _realtimeService.VoiceNoteProcessedAsync( voiceNote.Message.ConversationId, voiceNote.MessageId,cancellationToken);
            }
            catch (Exception ex)
            {
                await MarkAsFailedAsync(file, ex);

                throw;
            }
        }
        //thumbnail
        
        private async Task CreateThumbnailAsync(StoredFile originalFile, MediaProcessingResultDto result, CancellationToken cancellationToken)
        {
            if (result.ThumbnailStream is null)
            {
                return;
            }

            // Hangfire retry idempotency
            if (originalFile.ThumbnailFileId.HasValue)
            {
                return;
            }

            if (result.ThumbnailStream.CanSeek)
            {
                result.ThumbnailStream.Position = 0;
            }

            var thumbnail = await _storageService
                    .UploadGeneratedAsync(
                        result.ThumbnailStream,
                        $"{Guid.NewGuid():N}.jpg",
                        result.ThumbnailContentType
                            ?? "image/jpeg",
                        FileCategory.Thumbnail,
                        MediaType.Image,
                        originalFile.UploadedById,
                        cancellationToken);

            originalFile.ThumbnailFileId = thumbnail.Id;

            originalFile.ThumbnailFile = thumbnail;
        }

        //failed processing
        private async Task MarkAsFailedAsync(StoredFile file, Exception exception)
        {
            file.ProcessingStatus = FileProcessingStatus.Failed;

            file.ProcessingCompletedAt = null;

            file.ProcessingError = TruncateError(exception.Message);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception saveException)
            {
                _logger.LogError(saveException, "Failed to save processing failure for file {FileId}.", file.Id);
            }
        }


        private static string TruncateError(string error)
        {
            const int maxLength = 2000;

            if (string.IsNullOrWhiteSpace(error))
            {
                return "Unknown media processing error.";
            }

            return error.Length <= maxLength ? error : error[..maxLength];
        }
    }
}