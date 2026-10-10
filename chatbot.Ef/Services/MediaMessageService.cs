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
        private const int MaxFiles = 10;

        private static MessageType GetMessageType(
            IReadOnlyCollection<IFormFile> files)
        {
            if (files.Count == 0)
                return MessageType.Text;

            // Mixed file types are treated as a generic file message.
            var types = files
                .Select(f => f.ContentType.Split('/')[0])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (types.Count > 1)
                return MessageType.File;

            return types[0].ToLowerInvariant() switch
            {
                "image" => MessageType.Image,
                "video" => MessageType.Video,
                "audio" => MessageType.Audio,
                _ => MessageType.File
            };
        }
        private async Task<MessageDto> MapToMessageDtoAsync(Message message,CancellationToken cancellationToken = default)
        {
            var dto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content,
                MessageType = message.MessageType,
                CreatedAt = message.SendAt,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                IsDeleted = message.IsDeleted
            };

            foreach (var file in message.Files)
            {
                var fileUrl = await storageService.GetFileUrlAsync(
                    file.Id,
                    cancellationToken);

                dto.Files.Add(new FileMetadataDto
                {
                    Id = file.Id,
                    OriginalName = file.OriginalName,
                    ContentType = file.ContentType,
                    Size = file.Size,
                    FileUrl = fileUrl ?? string.Empty,
                    ThumbnailUrl = file.ThumbnailUrl,
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
            var isMember =await unitOfWork.ConversationMember.ExistsAsync(dto.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }
            cancellationToken.ThrowIfCancellationRequested();
            //create message
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ConversationId = dto.ConversationId,
                Content = dto.Content ?? string.Empty,
                MessageType = GetMessageType(dto.Files),
                SendAt = DateTime.UtcNow
            };

            await unitOfWork.Messages.AddAsync(message);

            var uploadedFiles = storageService.UploadMessageFilesAsync(dto.Files,message.Id,userId,cancellationToken);
            await unitOfWork.SaveChangesAsync();

            return await MapToMessageDtoAsync(message,cancellationToken);
        }
    }
}
