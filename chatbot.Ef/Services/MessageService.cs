using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public class MessageService(IUnitOfWork unitOfWork,IStorageService storage,IMessageStatusService messageStatusService
        ,IFileValidationService validator,IMapper mapper) : IMessageService
    {
        public async Task<DeleteMessageResultDto> DeleteForEveryoneAsync(Guid messageId, Guid userId)
        {
            var message =  await unitOfWork.Messages
               .GetByIdAsync(messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            var isMember = await unitOfWork.ConversationMember
                 .ExistsAsync(message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            if (message.SenderId != userId)
            {
                throw new UnauthorizedAccessException("Only the sender can delete this message.");
            }
            if (message.IsDeletedForEveryone)
            {
                return new DeleteMessageResultDto
                {
                    MessageId = message.Id,
                    ConversationId = message.ConversationId,
                    DeletedByUserId = userId,
                    DeletedAt = message.DeletedForEveryoneAt
                        ?? DateTime.UtcNow
                };
            }


            var deletedAt = DateTime.UtcNow;
            message.IsDeletedForEveryone = true;
            message.DeletedForEveryoneAt = deletedAt;
            message.Content = string.Empty;

            await unitOfWork.SaveChangesAsync();

            return new DeleteMessageResultDto
            {
                MessageId = message.Id,
                ConversationId = message.ConversationId,
                DeletedByUserId = userId,
                DeletedAt = deletedAt
            };

        }

        public async Task<DeleteForMeResultDto> DeleteForMeAsync(Guid messageId, Guid userId)
        {
            var message =  await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            var isMember = await unitOfWork.ConversationMember
                 .ExistsAsync(message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            var exists =await unitOfWork.Messages
                    .IsDeletedForUserAsync( messageId,userId);

            if (exists)
            {
                return new DeleteForMeResultDto
                {
                    MessageId = messageId,
                    UserId = userId,
                    DeletedAt = DateTime.UtcNow
                };
            }

            var deletedAt = DateTime.UtcNow;
            var deletion =
                new MessageDeletion
                {
                    MessageId = messageId,
                    UserId = userId,
                    DeletedAt = deletedAt
                };

            await unitOfWork.Messages.AddDeletionAsync(deletion);
            await unitOfWork.SaveChangesAsync();

            return new DeleteForMeResultDto
            {
                MessageId = messageId,
                UserId = userId,
                DeletedAt = deletedAt
            };
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
                .GetConversationMessagesAsync(conversationId,userId, page, pageSize);

            var totalCount = await unitOfWork.Messages.CountByConversationAsync(conversationId,userId);

            var items = mapper.Map<List<MessageDto>>(messages);

            return new PagedResultDto<MessageDto>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<MessageDto> ReplyAsync(Guid senderId, ReplyMessageDto dto)
        {
            var isMember = await unitOfWork.ConversationMember
                .ExistsAsync(dto.ConversationId, senderId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }
            var original = await unitOfWork.Messages
                .GetMessageByConversationIdAsync(dto.MessageId, dto.ConversationId);

            if (original is null)
            {
                throw new KeyNotFoundException("Original message not found.");
            }
            if (original.IsDeletedForEveryone)
            {
                throw new InvalidOperationException("You cannot reply to a deleted message.");
            }

            var deletedForUser = await unitOfWork.Messages
                .IsDeletedForUserAsync(original.Id, senderId);

            if (deletedForUser)
            {
                throw new KeyNotFoundException("Original message not found.");
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ConversationId = dto.ConversationId,
                Content = dto.Content,
                MessageType = dto.Type,
                SendAt = DateTime.UtcNow,
                ReplyToMessageId = original.Id

            };
           await unitOfWork.Messages.AddAsync(message);
            await unitOfWork.SaveChangesAsync();

            var recipientIds = await unitOfWork.ConversationMember
                .GetOtherMemberIdsAsync(dto.ConversationId, senderId);

            foreach (var recipientId in recipientIds)
            {
                await messageStatusService
                    .MarkAsSentAsync(message.Id, recipientId);
            }


            var senderName = original.Sender.UserName ?? string.Empty;

            return new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = senderName,
                Content = message.Content,
                MessageType = message.MessageType,
                CreatedAt = message.SendAt,
                ReplyToMessageId = original.Id,
                ReplyToMessage =
                new ReplyPreviewDto
                {
                    Id = original.Id,
                    SenderId = original.SenderId,
                    Content = original.Content,
                    MessageType = original.MessageType,
                    IsDeletedForEveryone = original.IsDeletedForEveryone
                }
            };
        }

        public Task<MessageDto> SendFileAsync(Guid senderId, SendFileDto filedto)
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
            var recipientIds = await unitOfWork.ConversationMember.GetOtherMemberIdsAsync(dto.ConversationId,senderId);

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
                MessageType = message.MessageType,
                CreatedAt = message.SendAt,
                IsEdited = false,
                IsDeletedForEveryone = false,
                IsForwarded = false,
                Files = new List<FileMetadataDto>(),
                Reactions = new List<MessageReactionDto>()
            };
        }
        private async Task<string> GetSenderNameAsync(Guid senderId)
        {
            var user = await unitOfWork.Auth.GetByIdAsync(senderId);

            return user?.UserName ?? string.Empty;
        }

    }
}
