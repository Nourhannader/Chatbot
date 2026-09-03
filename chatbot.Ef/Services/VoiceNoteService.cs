using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class VoiceNoteService(IUnitOfWork unitOfWork , IStorageService storageService,IFileValidationService validationService) : IVoiceNoteService
    {
        public async Task<MessageDto> SendAsync(
        SendVoiceNoteDto dto,
        Guid userId,
        CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //validate Conversation
            var isMember=await unitOfWork.Conversations.IsMemberAsync(Guid.Parse(dto.ConversationId), userId, cancellationToken);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            //validate file
           await validationService.ValidateFile(dto.Audio);

            var message = new Message
            {
                Id = Guid.NewGuid(),

                ConversationId = Guid.Parse(dto.ConversationId),

                SenderId = userId,

                Content = string.Empty,

                MessageType = MessageType.VoiceNote,

                SendAt = DateTime.UtcNow
            };
            await unitOfWork.Messages.AddAsync(message);


            var upload = await storageService.UploadAsync(
            dto.Audio,
            folder: "voice-notes",
            uploadedBy: userId,
            messageId: message.Id,
            cancellationToken: cancellationToken);


            var voiceNote = new VoiceNote
            {
                Id = Guid.NewGuid(),

                MessageId = message.Id,

                FileId = upload.FileId,

                DurationSeconds = dto.DurationSeconds,

                Waveform = GenerateWaveform(),

                CreatedAt = DateTime.UtcNow
            };


            //await unitOfWork.VoiceNotes.AddAsync(voiceNote);

            await unitOfWork.SaveChangesAsync();


            
            return new MessageDto
            {
                Id = message.Id.ToString(),
                ConversationId = message.ConversationId.ToString(),
                SenderId = message.SenderId.ToString(),
                Content = message.Content,
                MessageType = message.MessageType,
                CreatedAt = message.SendAt,

                Files = new List<FileMetadataDto>
                {
                new FileMetadataDto
                {
                    Id = upload.FileId.ToString(),
                    OriginalName = dto.Audio.FileName,
                    ContentType = upload.ContentType,
                    Size = upload.Size,
                    FileUrl = upload.FileUrl,
                    ThumbnailUrl = upload.ThumbnailUrl,
                    DurationSeconds = dto.DurationSeconds
                }
                },

                VoiceNote = new VoiceNoteDto
                {
                    Id = voiceNote.Id.ToString(),
                    FileId = voiceNote.FileId.ToString(),
                    DurationSeconds = voiceNote.DurationSeconds,
                    Waveform = voiceNote.Waveform
                },

                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                IsDeleted = message.IsDeleted
            };
        }


        private string GenerateWaveform()
        {
            return "12,25,40,30,20,50,80";
        }


    }
}
