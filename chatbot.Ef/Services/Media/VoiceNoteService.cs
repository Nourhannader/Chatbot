using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Services
{
    public class VoiceNoteService : IVoiceNoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IMediaDtoEnricher _dtoEnricher;
        private readonly IStorageService _storageService;
        private readonly OutboxMessageFactory _outboxMessageFactory;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<VoiceNoteService> _logger;

        public VoiceNoteService(
            IUnitOfWork unitOfWork,
            IFileValidationService validationService,
            IMapper mapper,
            IMediaDtoEnricher dtoEnricher,
            IStorageService storageService,
            OutboxMessageFactory outboxMessageFactory,
            TimeProvider timeProvider,
            ILogger<VoiceNoteService> logger)
        {
            _unitOfWork = unitOfWork;
            _validationService = validationService;
            _mapper = mapper;
            _dtoEnricher = dtoEnricher;
            _storageService = storageService;
            _outboxMessageFactory = outboxMessageFactory;
            _timeProvider = timeProvider;
            _logger = logger;
        }


        public async Task<MessageDto> SendAsync(SendVoiceNoteDto dto,Guid userId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (dto.ConversationId == Guid.Empty)
                throw new ArgumentException("ConversationId is required.");


            var isMember = await _unitOfWork.ConversationMember.ExistsAsync(dto.ConversationId, userId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not an active member of this conversation.");

            // Validate reply
            if (dto.ReplyToMessageId.HasValue)
            {
                var replyMessage = await _unitOfWork.Messages.GetByIdAsync(dto.ReplyToMessageId.Value);

                if (replyMessage is null ||
                    replyMessage.ConversationId != dto.ConversationId ||
                    replyMessage.IsDeletedForEveryone)
                {
                    throw new NotFoundException("Reply message was not found in this conversation.");
                }
            }

            var validated = await _validationService.ValidateVoiceAsync(dto.File, cancellationToken);

            StoredFile? uploadedFile = null;
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ConversationId = dto.ConversationId,
                Content = string.Empty,
                ReplyToMessageId = dto.ReplyToMessageId,
                MessageType = MessageType.Voice,
                SendAt = now
            };
            var voiceNote = new VoiceNote
            {
                Id = Guid.NewGuid(),
                MessageId = message.Id,
                Message = message,
                DurationSeconds = 0,
                Waveform = "[]",
                CreatedAt = now
            };

            try
            {
                uploadedFile = await _storageService.UploadAsync(
                    dto.File,
                    validated,
                    FileCategory.VoiceNote,
                    message.Id,
                    userId,
                    cancellationToken);

                voiceNote.FileId = uploadedFile.Id;
                voiceNote.File = uploadedFile;

                message.VoiceNote = voiceNote;

                await _unitOfWork.Messages.AddAsync(message);

                var createdEvent = new MessageCreatedEvent( message.Id, message.ConversationId, message.SenderId,
                    message.Content, message.MessageType,
                    message.ReplyToMessageId, message.SendAt);

                await _unitOfWork.OutboxMessages.AddAsync(
                    _outboxMessageFactory.Create(createdEvent));

                if (uploadedFile.ProcessingStatus == FileProcessingStatus.Pending)
                {
                    var processingEvent = new VoiceNoteProcessingRequestedEvent(voiceNote.Id, message.Id);

                    await _unitOfWork.OutboxMessages.AddAsync(
                        _outboxMessageFactory.Create(processingEvent));
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                if (uploadedFile is not null)
                {
                    try
                    {
                        await _storageService.DeleteAsync(uploadedFile,CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,"Failed to compensate voice file {FileId}.", uploadedFile.Id);
                    }
                }

                throw;
            }

            var result = _mapper.Map<MessageDto>(message);

            await _dtoEnricher.EnrichAsync( message,result,cancellationToken);

            return result;
        }


    }
}
