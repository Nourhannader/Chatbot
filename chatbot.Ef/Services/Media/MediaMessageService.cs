using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using chatbot.Core.Events.Media;
using chatbot.Core.Events.Messages;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.Media;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.Outbox;
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
        private readonly IStorageService _storageService;
        private readonly IMediaJobDispatcher _jobDispatcher;
        private readonly IMapper _mapper;
        private readonly IMediaDtoEnricher _dtoEnricher;
        private readonly ILogger<MediaMessageService> _logger;
        private readonly OutboxMessageFactory _outboxMessageFactory;
        private readonly TimeProvider _timeProvider;

        public MediaMessageService(
        IUnitOfWork unitOfWork,
        IFileValidationService validationService,
        IStorageService storageService,
        IMediaJobDispatcher jobDispatcher,
        IMapper mapper,
        IMediaDtoEnricher dtoEnricher,
        OutboxMessageFactory outboxMessageFactory,
        TimeProvider timeProvider,
        ILogger<MediaMessageService> logger)
        {
            _unitOfWork = unitOfWork;
            _validationService = validationService;
            _storageService = storageService;
            _jobDispatcher = jobDispatcher;
            _mapper = mapper;
            _dtoEnricher = dtoEnricher;
            _outboxMessageFactory = outboxMessageFactory;
            _logger = logger;
            _timeProvider = timeProvider;
        }

        
        public async Task<MessageDto> SendMediaAsync( SendMediaDto dto, Guid userId,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            cancellationToken.ThrowIfCancellationRequested();

            // 1. Validate files

            if (dto.ConversationId == Guid.Empty)
                throw new ArgumentException("ConversationId is required.");

            var files = dto.Files?.ToList() ?? throw new ArgumentException("At least one file is required.");

            if (files.Count == 0)
                throw new ArgumentException("At least one file is required.");

            if (files.Count > MaxFiles)
                throw new ArgumentException($"Maximum {MaxFiles} files are allowed.");

            if (files.Any(f => f is null || f.Length <= 0))
                throw new ArgumentException("Files must not be empty.");

            //2.check if the user is a member of the conversation
            var isMember =await _unitOfWork.ConversationMember.ExistsAsync(dto.ConversationId, userId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not an active member of this conversation.");

            // 3. Validate Reply


            if (dto.ReplyToMessageId.HasValue)
            {
                var replyMessage = await _unitOfWork.Messages.GetByIdAsync(dto.ReplyToMessageId.Value);

                if (replyMessage is null || replyMessage.ConversationId != dto.ConversationId ||
                          replyMessage.IsDeletedForEveryone)
                {
                    throw new NotFoundException("Reply message was not found in this conversation.");
                }
            }
            //4.validated all files
            var validatedFiles = new List<ValidatedFileDto>(files.Count);
            foreach ( var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var validated= await _validationService.ValidateFileAsync(file, cancellationToken);
                validatedFiles.Add(validated);
            }
            //5.create message
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ConversationId = dto.ConversationId,
                Content = dto.Content?.Trim() ?? string.Empty,
                ReplyToMessageId = dto.ReplyToMessageId,
                MessageType = ResolveMessageType(validatedFiles),
                SendAt = now
            };
            // 6. Upload files

            var uploadedFiles =new List<StoredFile>();

            try
            {
                for (var index = 0; index < files.Count; index++)
                {
                    var file = files[index];

                    var validated = validatedFiles[index];


                    // -----------------------------------------
                    // StorageService:
                    //
                    // ownerId = message.Id
                    // uploadedById = current user
                    // -----------------------------------------

                    var storedFile = await _storageService .UploadAsync(file, validated,FileCategory.MessageFile, message.Id, userId, cancellationToken);

                    uploadedFiles.Add(storedFile);


                    // -----------------------------------------
                    // MessageAttachment
                    // -----------------------------------------

                    var attachment = new MessageAttachment
                    {
                        Id = Guid.NewGuid(),
                        MessageId = message.Id,
                        Message = message,
                        StoredFileId = storedFile.Id,
                        StoredFile = storedFile,
                        Order = index
                    };

                    message.Attachments.Add(attachment);
                }
                // 7. Save Message

                await _unitOfWork.Messages.AddAsync(message);
                var messageCreatedEvent = new MessageCreatedEvent(message.Id, message.ConversationId, message.SenderId, message.Content,
                     message.MessageType, message.ReplyToMessageId, message.SendAt);

                await _unitOfWork.OutboxMessages.AddAsync(_outboxMessageFactory.Create(messageCreatedEvent));

                var pendingFileIds = uploadedFiles
                    .Where(x => x.ProcessingStatus == FileProcessingStatus.Pending)
                    .Select(x => x.Id)
                    .ToArray();

                if (pendingFileIds.Length > 0)
                {
                    var processingEvent = new MediaProcessingRequestedEvent(message.Id, pendingFileIds);

                    await _unitOfWork.OutboxMessages.AddAsync(_outboxMessageFactory.Create(processingEvent));
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // Compensation

                await CompensateAsync(uploadedFiles);

                throw;
            }




            // 8. Map response

            var result = _mapper.Map<MessageDto>(message);

            await _dtoEnricher.EnrichAsync(message, result, cancellationToken);


            return result;
        }

        private async Task CompensateAsync(IEnumerable<StoredFile> uploadedFiles)
        {
            foreach (var file in uploadedFiles)
            {
                try
                {
                    await _storageService.DeleteAsync(file, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Failed to compensate uploaded file {FileId} " + "with storage key {StorageKey}.", file.Id, file.StorageKey);
                }
            }
        }

        private static MessageType ResolveMessageType( IReadOnlyCollection<ValidatedFileDto> files)
        {
            if (files.Count == 0)
            {
                return MessageType.Text;
            }

            var mediaTypes =files .Select(x => x.MediaType).Distinct().ToList();


            // Mixed media
            if (mediaTypes.Count != 1)
            {
                return MessageType.File;
            }


            return mediaTypes[0] switch
            {
                MediaType.Image =>MessageType.Image,

                MediaType.Video => MessageType.Video,

                MediaType.Audio =>MessageType.Audio,

                MediaType.Document => MessageType.Document,

                _ =>
                    MessageType.File
            };
        }


        public async Task<MediaDownloadResultDto> GetAttachmentAsync(Guid messageId, Guid attachmentId, Guid userId, CancellationToken cancellationToken = default)
        {

        var attachment = await _unitOfWork.MessageAttachments.GetWithFileAsync(attachmentId);

            if (attachment is null || attachment.MessageId != messageId)
                throw new NotFoundException("Attachment was not found.");

            var message = await _unitOfWork.Messages.GetByIdAsync(messageId);

            if (message is null || message.IsDeletedForEveryone)
                throw new NotFoundException("Message was not found.");


            var isMember = await _unitOfWork.ConversationMember.ExistsAsync(message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not allowed to access this attachment.");
            }


            var storedFile = attachment.StoredFile;

            if (storedFile is null || storedFile.IsDeleted || storedFile.IsPhysicallyDeleted)
            {
                throw new NotFoundException("File is no longer available.");
            }

            var stream = await _storageService.DownloadAsync( storedFile.StorageKey, storedFile.Provider, cancellationToken);


            return new MediaDownloadResultDto
            {
                Stream = stream,

                ContentType = storedFile.ContentType,

                FileName = storedFile.OriginalFileName,

                FileSize = storedFile.FileSize
            };
        }
    }
}
