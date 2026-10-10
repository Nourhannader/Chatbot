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
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public class MessageService(IUnitOfWork unitOfWork,IStorageService storage,IMessageStatusService messageStatusService,IFileValidationService validator) : IMessageService
    {
        public async Task DeleteForEveryoneAsync(Guid messageId, Guid userId)
        {
            var message =
           await unitOfWork.Messages
               .GetByIdAsync(messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            if (message.SenderId != userId)
                throw new UnauthorizedAccessException("Only sender can delete this message.");

            if (message.IsDeletedForEveryone)
                return;

            message.IsDeletedForEveryone = true;

            message.DeletedForEveryoneAt =
                DateTime.UtcNow;

            message.Content = "This message was deleted";

            await unitOfWork
                .SaveChangesAsync();

        }

        public async Task DeleteForMeAsync(Guid messageId, Guid userId)
        {
            var message =
            await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            var exists =
                await unitOfWork.Messages
                    .IsDeletedForUserAsync(
                        messageId,
                        userId);

            if (exists)
                return;

            var deletion = new MessageDeletion
            {
                MessageId = messageId,
                UserId = userId,
                DeletedAt = DateTime.UtcNow
            };

            await unitOfWork.Messages
                .AddDeletionAsync(deletion);

            await unitOfWork
                .SaveChangesAsync();
        }

        public async Task<PagedResultDto<MessageDto>> GetMessagesAsyns(Guid userId, Guid conversationId, int page = 1, int pageSize = 20)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be between 1 and 100.");

            var isMember = await unitOfWork.ConversationMember.ExistsAsync(conversationId, userId);
            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this conversation.");

            var messages = await unitOfWork.Messages
                .GetConversationMessagesAsync(conversationId, page, pageSize);

            var totalCount = await unitOfWork.Messages.CountByConversationAsync(conversationId);

            var items = messages.Select(message => new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender.UserName ?? string.Empty,
                Content = message.Content,
                CreatedAt = message.SendAt,

                Files = message.Files.Select(file => new FileMetadataDto
                {
                    Id = file.Id,
                    OriginalName = file.OriginalName,
                    ContentType = file.ContentType,
                    Size = file.Size,
                    FileUrl = file.CDNUrl,
                    ThumbnailUrl = file.ThumbnailUrl,
                    Width = file.Width,
                    Height = file.Height,
                    DurationSeconds = file.DurationSeconds


                }).ToList()
            }).ToList();

            return new PagedResultDto<MessageDto>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Message> ReplyAsync(Guid senderId, ReplyMessageDto dto)
        {
            var originalMessage = await unitOfWork.Messages.GetByIdAsync(dto.MessageId);
            if (originalMessage == null)
                throw new Exception("Message not found.");
            if (originalMessage.ConversationId != dto.ConversationId)
                throw new InvalidOperationException("The original message does not belong to the this conversation.");

            var message = new Message
            {
                SenderId = senderId,
                ConversationId = dto.ConversationId,
                Content = dto.Content,
                MessageType = dto.Type,
                SendAt = DateTime.UtcNow,
                ReplyToMessageId = dto.MessageId

            };
           await unitOfWork.Messages.AddAsync(message);
            await unitOfWork.SaveChangesAsync();
            return message;
        }

        public Task<MessageDto> SendFileAsync(SendFileDto filedto)
        {
           // validator.ValidateFile(filedto.)
           throw new NotImplementedException();
        }

        public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto dto,
          CancellationToken cancellationToken = default)
        {
            if (dto.ConversationId == Guid.Empty)
                throw new ArgumentException("ConversationId is required.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException("Message content cannot be empty.");

            var isMember = await unitOfWork.ConversationMember.ExistsAsync(dto.ConversationId, senderId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this conversation.");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = dto.ConversationId,
                SenderId = senderId,
                Content = dto.Content.Trim(),
                MessageType = MessageType.Text,
                SendAt = DateTime.UtcNow
            };

            
            await unitOfWork.Messages.AddAsync(message);
            await unitOfWork.SaveChangesAsync();
            var recipientIds =
            await unitOfWork.ConversationMember.GetOtherMemberIdsAsync(dto.ConversationId,senderId);

            foreach (var recipientId in recipientIds)
            {
                await messageStatusService.MarkAsSentAsync(message.Id,recipientId);
            }

            var senderName = await GetSenderNameAsync(senderId);

            return new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = senderName,
                Content = message.Content,
                CreatedAt = message.SendAt
            };
        }
        private async Task<string> GetSenderNameAsync( Guid senderId)
        {
            var user = await unitOfWork.Auth.GetByIdAsync(senderId);

            return user?.UserName ?? string.Empty;
        }

    }
}
