using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public class MediaMessageService(IUnitOfWork unitOfWork,IStorageService storageService) : IMediaMessageService
    {
        private static MessageType GetMessageType(List<IFormFile> files)
        {
            if (files.Count == 0)
                return MessageType.Text;

            var file = files.First();

            if (file.ContentType.StartsWith("image/"))
                return MessageType.Image;

            if (file.ContentType.StartsWith("video/"))
                return MessageType.Video;

            if (file.ContentType.StartsWith("audio/"))
                return MessageType.Audio;

            return MessageType.File;
        }

        private async Task<MessageDto> MapToMessageDtoAsync(Message message,CancellationToken cancellationToken = default)
        {
            var dto = new MessageDto
            {
                Id = message.Id.ToString(),
                ConversationId = message.ConversationId.ToString(),
                SenderId = message.SenderId.ToString(),
                Content = message.Content,
                MessageType = message.MessageType,
                CreatedAt = message.SendAt,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                IsDeleted = message.IsDeleted
            };

            foreach (var file in message.StoredFiles)
            {
                var fileUrl = await storageService.GetFileUrlAsync(
                    file.Id,
                    cancellationToken);

                dto.Files.Add(new FileMetadataDto
                {
                    Id = file.Id.ToString(),
                    OriginalName = file.OriginalName,
                    ContentType = file.ContentType,
                    Size = file.Size,
                    FileUrl = fileUrl ?? string.Empty,
                    ThumbnailUrl = file.ThumbnailPath,
                    Width = file.Width,
                    Height = file.Height,
                    DurationSeconds = file.DurationSeconds
                });
            }

            if (message.VoiceNote != null)
            {
                dto.VoiceNote = new VoiceNoteDto
                {
                    Id = message.VoiceNote.Id.ToString(),
                    FileId = message.VoiceNote.FileId.ToString(),
                    DurationSeconds =
                        message.VoiceNote.DurationSeconds,
                    Waveform =
                        message.VoiceNote.Waveform
                };
            }

            return dto;
        }
        public async Task<MessageDto> SendMediaAsync( SendMediaDto dto, Guid userId,CancellationToken cancellationToken = default)
        {
           cancellationToken.ThrowIfCancellationRequested();
            //check if the user is a member of the conversation
            var isMember =await unitOfWork.Conversations.IsMemberAsync(dto.ConversationId, userId, cancellationToken);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }
            //check files are not null or empty
            if (dto.Files == null || !dto.Files.Any())
            {
                throw new ArgumentException(
                    "At least one file is required.");
            }
            //create message
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ConversationId = dto.ConversationId,
                Content = dto.Caption ?? string.Empty,
                MessageType = GetMessageType(dto.Files),
                SendAt = DateTime.UtcNow
            };

            await unitOfWork.Messages.AddAsync(message);

            var uploadedFiles = storageService.UploadManyAsync(dto.Files,message.Id,"messages",userId,cancellationToken);
            await unitOfWork.SaveChangesAsync();

            return await MapToMessageDtoAsync(message,cancellationToken);
        }
    }
}
