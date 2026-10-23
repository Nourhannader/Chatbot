using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.Media;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Services
{
    public class MediaMessageService : IMediaMessageService
    {
        private const int MaxFiles = 10;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileValidationService _validationService;
        private readonly IMediaStorageService _mediaStorage;
        private readonly IMediaJobDispatcher _jobDispatcher;
        private readonly IMapper _mapper;
        private readonly IMediaDtoEnricher _dtoEnricher;
        private readonly IChatRealtimeService _realtimeService;
        private readonly ILogger<MediaMessageService> _logger;

        public MediaMessageService(
        IUnitOfWork unitOfWork,
        IFileValidationService validationService,
        IMediaStorageService mediaStorage,
        IMediaJobDispatcher jobDispatcher,
        IMapper mapper,
        IMediaDtoEnricher dtoEnricher,
        IChatRealtimeService realtimeService,
        ILogger<MediaMessageService> logger)
        {
            _unitOfWork = unitOfWork;
            _validationService = validationService;
            _mediaStorage = mediaStorage;
            _jobDispatcher = jobDispatcher;
            _mapper = mapper;
            _dtoEnricher = dtoEnricher;
            _realtimeService = realtimeService;
            _logger = logger;
        }

        
        public async Task<MessageDto> SendMediaAsync( SendMediaDto dto, Guid userId,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            cancellationToken.ThrowIfCancellationRequested();

            // 1. Validate files
            var files = dto.Files?.ToList() ?? throw new ArgumentException("At least one file is required.");

            if (files.Count == 0)
                throw new ArgumentException("At least one file is required.");

            if (files.Count > MaxFiles)
                throw new ArgumentException($"Maximum {MaxFiles} files are allowed.");

            if (files.Any(f => f is null || f.Length <= 0))
                throw new ArgumentException("Files must not be empty.");

            //check if the user is a member of the conversation
            var isMember =await _unitOfWork.ConversationMember.ExistsAsync(dto.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }
            //validated all files
            var validatedFiles = new List<ValidatedFileDto>(files.Count);
            foreach ( var file in files)
            {
               var result= await _validationService.ValidateFileAsync(file, cancellationToken);
                validatedFiles.Add(result);
            }
            //create message
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ConversationId = dto.ConversationId,
                Content = dto.Content?.Trim() ?? string.Empty,
                ReplyToMessageId = dto.ReplyToMessageId,
                MessageType = ResolveMessageType(validatedFiles),
                SendAt = DateTime.UtcNow
            };

            await _unitOfWork.Messages.AddAsync(message);

            var uploadedFiles = new List<StoredFile>();

            try
            {
                // ==========================
                // Upload
                // ==========================

                for (var index = 0; index < files.Count; index++)
                {
                    var validated = validatedFiles[index];

                    var storedFile = await _mediaStorage.UploadAsync(files[index], validated,
                            userId, BuildFolder(validated.Category), cancellationToken);
                    uploadedFiles.Add(storedFile);
                   // await _unitOfWork.StoredFiles.AddAsync(storedFile);

                    var attachment =
                        new MessageAttachment
                        {
                            Id = Guid.NewGuid(),
                            MessageId = message.Id,
                            StoredFileId = storedFile.Id,
                            StoredFile = storedFile,
                            Order = index
                        };

                    message.Attachments.Add(attachment);
                }
                await _unitOfWork.StoredFiles.AddRangeAsync(uploadedFiles);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                await CompensateAsync(uploadedFiles);

                throw;
            }

            // ==========================
            // Queue processing
            // ==========================

            foreach (var file in uploadedFiles)
            {
                if (file.ProcessingStatus == FileProcessingStatus.Pending)
                {
                    _jobDispatcher.EnqueueAttachmentProcessing(file.Id);
                }
            }

            var resultDto = _mapper.Map<MessageDto>(message);

            await _dtoEnricher.EnrichAsync(message, resultDto, cancellationToken);

            await _realtimeService.MessageSentAsync(message.ConversationId, resultDto, cancellationToken);

            return resultDto;
        }

        private async Task CompensateAsync(IEnumerable<StoredFile> uploadedFiles)
        {
            foreach (var file in uploadedFiles)
            {
                try
                {
                    await _mediaStorage.DeleteAsync(file, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to compensate uploaded file {FileId} with key {StorageKey}.",
                        file.Id, file.StorageKey);
                }
            }
        }

        private static MessageType ResolveMessageType(IReadOnlyCollection<ValidatedFileDto> files)
        {
            var categories = files.Select(x => x.Category).Distinct().ToList();

            if (categories.Count != 1)
                return MessageType.File;

            return categories[0] switch
            {
                FileCategory.Image => MessageType.Image,
                FileCategory.Video => MessageType.Video,
                FileCategory.Audio => MessageType.Audio,
                FileCategory.Document => MessageType.Document,
                _ =>
                    MessageType.File
            };
        }

        private static string BuildFolder(FileCategory category)
        {
            var categoryFolder = category switch
            {
                FileCategory.Image => "images",
                FileCategory.Video => "videos",
                FileCategory.Audio => "audio",
                FileCategory.Document => "documents",
                _ => "files"
            };

            return
                $"messages/{categoryFolder}/{DateTime.UtcNow:yyyy/MM}";
        }
    }
}
