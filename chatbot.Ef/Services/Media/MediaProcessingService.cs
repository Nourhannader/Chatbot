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
using chatbot.Ef.Outbox;
using chatbot.Core.Events.Media;

namespace chatbot.Ef.Services.Media
{
    public class MediaProcessingService : IMediaProcessingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IFileProcessorFactory _processorFactory;
        private readonly OutboxMessageFactory _outboxMessageFactory;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<MediaProcessingService> _logger;

        public MediaProcessingService(
            IUnitOfWork unitOfWork,
            IStorageService storageService,
            IFileProcessorFactory processorFactory,
            OutboxMessageFactory outboxMessageFactory,
            TimeProvider timeProvider,
            ILogger<MediaProcessingService> logger)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _processorFactory = processorFactory;
            _outboxMessageFactory = outboxMessageFactory;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task ProcessAttachmentAsync(Guid storedFileId, CancellationToken cancellationToken = default)
        {
            var file = await _unitOfWork.StoredFiles.GetForProcessingAsync(storedFileId);

            if (file is null || file.IsDeleted || file.IsPhysicallyDeleted)
                return;

            if (file.ProcessingStatus == FileProcessingStatus.Completed ||
                file.ProcessingStatus == FileProcessingStatus.NotRequired)
            {
                return;
            }

            // Documents don't require media processing
            if (file.MediaType is MediaType.Document or MediaType.Other)
            {
                file.ProcessingStatus = FileProcessingStatus.NotRequired;
                file.ProcessingCompletedAt = _timeProvider.GetUtcNow().UtcDateTime;
                file.ProcessingError = null;

                await _unitOfWork.SaveChangesAsync();
                return;
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            file.ProcessingStatus = FileProcessingStatus.Processing;
            file.ProcessingStartedAt = now;
            file.ProcessingCompletedAt = null;
            file.ProcessingError = null;

            await _unitOfWork.SaveChangesAsync();

            StoredFile? generatedThumbnail = null;

            try
            {
                await using var stream = await _storageService.DownloadAsync(file.StorageKey,file.Provider,cancellationToken)
                    ?? throw new FileNotFoundException( $"Stored file '{file.Id}' was not found.");

                var processor = _processorFactory.GetProcessor(file.MediaType);

                var result = await processor.ProcessAsync( stream,file.ContentType,cancellationToken);

                file.Width = result.Width;
                file.Height = result.Height;
                file.DurationSeconds = result.DurationSeconds;

                if (result.ThumbnailStream is not null)
                {
                    generatedThumbnail = await CreateThumbnailAsync( file, result, cancellationToken);
                }

                file.ProcessingStatus = FileProcessingStatus.Completed;
                file.ProcessingCompletedAt = _timeProvider.GetUtcNow().UtcDateTime;
                file.ProcessingError = null;

                var attachment = file.MessageAttachments.FirstOrDefault();

                if (attachment is not null)
                {
                    var processedEvent = new MediaProcessedEvent(
                        attachment.MessageId,attachment.Message.ConversationId,
                        file.Id,
                        file.Width,
                        file.Height,
                        file.DurationSeconds,
                        file.ThumbnailFileId,
                        file.ProcessingCompletedAt.Value);

                    await _unitOfWork.OutboxMessages.AddAsync(_outboxMessageFactory.Create(processedEvent));
                }

                // Metadata + thumbnail relationship + Outbox
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // If thumbnail upload succeeded but DB save failed,
                // the thumbnail may be an orphan. Try to delete it.
                if (generatedThumbnail is not null)
                {
                    try
                    {
                        await _storageService.DeleteAsync( generatedThumbnail,CancellationToken.None);
                    }
                    catch (Exception deleteException)
                    {
                        _logger.LogError( deleteException, "Failed to delete generated thumbnail {ThumbnailId}.",generatedThumbnail.Id);
                    }
                }

                await MarkAsFailedAsync(file, ex);
                throw;
            }
        }

        public async Task ProcessVoiceNoteAsync( Guid voiceNoteId,CancellationToken cancellationToken = default)
        {
            var voiceNote = await _unitOfWork.VoiceNotes
                .GetForProcessingAsync(voiceNoteId);

            if (voiceNote is null)
                return;

            var file = voiceNote.File;

            if (file is null || file.IsDeleted || file.IsPhysicallyDeleted)
                return;

            if (file.ProcessingStatus == FileProcessingStatus.Completed)
                return;

            if (file.MediaType != MediaType.Audio)
                throw new InvalidOperationException("Voice note file must be an audio file.");

            file.ProcessingStatus = FileProcessingStatus.Processing;
            file.ProcessingStartedAt = _timeProvider.GetUtcNow().UtcDateTime;
            file.ProcessingCompletedAt = null;
            file.ProcessingError = null;

            await _unitOfWork.SaveChangesAsync();

            try
            {
                await using var stream = await _storageService.DownloadAsync(file.StorageKey, file.Provider,cancellationToken)
                    ?? throw new FileNotFoundException( $"Voice file '{file.Id}' was not found.");

                var processor = _processorFactory.GetProcessor(MediaType.Audio);

                var result = await processor.ProcessAsync(stream,file.ContentType,cancellationToken);

                var duration = result.DurationSeconds ?? 0;

                file.DurationSeconds = duration;

                voiceNote.DurationSeconds = (int)Math.Round(duration, MidpointRounding.AwayFromZero);

                voiceNote.Waveform = JsonSerializer.Serialize(result.Waveform ?? new List<double>());

                file.ProcessingStatus = FileProcessingStatus.Completed;
                file.ProcessingCompletedAt = _timeProvider.GetUtcNow().UtcDateTime;
                file.ProcessingError = null;

                var processedEvent = new VoiceNoteProcessedEvent( voiceNote.MessageId,voiceNote.Message.ConversationId, voiceNote.Id,voiceNote.DurationSeconds,
                    voiceNote.Waveform, file.ProcessingCompletedAt.Value);

                await _unitOfWork.OutboxMessages.AddAsync(_outboxMessageFactory.Create(processedEvent));

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await MarkAsFailedAsync(file, ex);
                throw;
            }
        }


        //thumbnail

        private async Task<StoredFile> CreateThumbnailAsync(StoredFile originalFile, MediaProcessingResultDto result,CancellationToken cancellationToken)
        {
            if (result.ThumbnailStream is null)
                throw new InvalidOperationException("Thumbnail stream is missing.");

            if (result.ThumbnailStream.CanSeek)
                result.ThumbnailStream.Position = 0;

            var thumbnail = await _storageService.UploadGeneratedAsync(
                result.ThumbnailStream,
                $"{Guid.NewGuid():N}.jpg",
                result.ThumbnailContentType ?? "image/jpeg",
                FileCategory.Thumbnail,
                MediaType.Image,
                originalFile.UploadedById,
                cancellationToken);

            originalFile.ThumbnailFileId = thumbnail.Id;
            originalFile.ThumbnailFile = thumbnail;

            return thumbnail;
        }

        //failed processing
        private async Task MarkAsFailedAsync(StoredFile file, Exception exception)
        {
            file.ProcessingStatus = FileProcessingStatus.Failed;
            file.ProcessingCompletedAt = null;
            file.ProcessingError = TruncateError(exception.Message);

            try
            {
                var attachment = file.MessageAttachments.FirstOrDefault();

                if (attachment is not null)
                {
                    var failedEvent = new MediaProcessingFailedEvent(file.Id,attachment.MessageId, attachment.Message.ConversationId,
                        file.ProcessingError,_timeProvider.GetUtcNow().UtcDateTime);

                    await _unitOfWork.OutboxMessages.AddAsync( _outboxMessageFactory.Create(failedEvent));
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception saveException)
            {
                _logger.LogError(saveException,"Failed to save processing failure for file {FileId}.", file.Id);
            }
        }


        private static string TruncateError(string error)
        {
            const int maxLength = 2000;

            if (string.IsNullOrWhiteSpace(error))
                return "Unknown media processing error.";

            return error.Length <= maxLength ? error : error[..maxLength];
        }
    }
}