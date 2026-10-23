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
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Services
{
    public class VoiceNoteService : IVoiceNoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileValidationService _validationService;
        private readonly IMediaStorageService _mediaStorage;
        private readonly IMediaJobDispatcher _jobDispatcher;
        private readonly IMapper _mapper;
        private readonly IMediaDtoEnricher _dtoEnricher;
        private readonly IChatRealtimeService _realtimeService;
        private readonly ILogger<VoiceNoteService> _logger;

        public VoiceNoteService(
            IUnitOfWork unitOfWork,
            IFileValidationService validationService,
            IMediaStorageService mediaStorage,
            IMediaJobDispatcher jobDispatcher,
            IMapper mapper,
            IMediaDtoEnricher dtoEnricher,
            IChatRealtimeService realtimeService,
            ILogger<VoiceNoteService> logger)
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


        public async Task<MessageDto> SendAsync(SendVoiceNoteDto dto,Guid userId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            //validate Conversation
            var isMember=await _unitOfWork.ConversationMember.ExistsAsync(dto.ConversationId,userId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            var validated = await _validationService.ValidateVoiceAsync(dto.File, cancellationToken);

            StoredFile? uploadedFile = null;

            try
            {
                uploadedFile = await _mediaStorage.UploadAsync(dto.File, validated, userId,
                        $"messages/voice/{DateTime.UtcNow:yyyy/MM}", cancellationToken);

                await _unitOfWork.StoredFiles.AddAsync(uploadedFile);

                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    SenderId = userId,
                    ConversationId = dto.ConversationId,
                    Content = string.Empty,
                    ReplyToMessageId = dto.ReplyToMessageId,
                    MessageType = MessageType.Voice,
                    SendAt = DateTime.UtcNow
                };

                var voiceNote = new VoiceNote
                {
                    Id = Guid.NewGuid(),
                    MessageId = message.Id,
                    Message = message,
                    FileId = uploadedFile.Id,
                    File = uploadedFile,
                    DurationSeconds = 0,
                    Waveform = "[]",
                    CreatedAt = DateTime.UtcNow
                };

                message.VoiceNote = voiceNote;
                await _unitOfWork.Messages.AddAsync(message);
                await _unitOfWork.VoiceNotes.AddAsync(voiceNote);
                await _unitOfWork.SaveChangesAsync();
                _jobDispatcher.EnqueueVoiceNoteProcessing(voiceNote.Id);
                var result = _mapper.Map<MessageDto>(message);

                await _dtoEnricher.EnrichAsync(message, result, cancellationToken);

                await _realtimeService.MessageSentAsync(message.ConversationId, result, cancellationToken);

                return result;
            }
            catch
            {
                if (uploadedFile != null)
                {
                    try
                    {
                        await _mediaStorage.DeleteAsync(uploadedFile, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to compensate voice file {FileId}.", uploadedFile.Id);
                    }
                }

                throw;
            }
        }


    }
}
