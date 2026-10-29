using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using chatbot.Core.Events.Messages;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.Outbox;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public class MessageService(IUnitOfWork unitOfWork,IStorageService storage,IMessageStatusService messageStatusService,
        IAuthorizationService authorizationService,ICurrentUser currentUser,OutboxMessageFactory outboxMessageFactory
        ,IFileValidationService validator,IMapper mapper) : IMessageService
    {
        public async Task<DeleteMessageResultDto> DeleteForEveryoneAsync(Guid messageId, Guid userId)
        {
            var message =  await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            if (message.SenderId != userId)
            {
                throw new UnauthorizedAccessException("Only the sender can delete this message.");
            }

            var resource = new ConversationResource(message.ConversationId, message.Id, message.SenderId);

            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser.Principal, resource, ConversationPermissions.DeleteOwnMessage);

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException("You cannot delete this message.");
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

            var @event = new MessageDeletedEvent(MessageId: message.Id, ConversationId: message.ConversationId,
               UserId:userId,MessageDeleteType.ForEveryone,DeletedAt:deletedAt );
            await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(@event));

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

            var exists =await unitOfWork.Messages.IsDeletedForUserAsync( messageId,userId);

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
            var deletion = new MessageDeletion
                {
                    MessageId = messageId,
                    UserId = userId,
                    DeletedAt = deletedAt
                };

            await unitOfWork.Messages.AddDeletionAsync(deletion);

            var @event = new MessageDeletedEvent(MessageId: message.Id, ConversationId: message.ConversationId,
              UserId: userId, MessageDeleteType.ForMe, DeletedAt: deletedAt);
            await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(@event));

            await unitOfWork.SaveChangesAsync();

            return new DeleteForMeResultDto
            {
                MessageId = messageId,
                UserId = userId,
                DeletedAt = deletedAt
            };
        }

        public async Task<MessageDto> EditAsync(Guid userId, Guid messageId, EditMessageDto dto)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
                throw new NotFoundException("Message not found.");

            if (message.IsDeletedForEveryone)
                throw new BadRequestException("Deleted message cannot be edited.");

            if (message.MessageType != MessageType.Text)
            {
                throw new BadRequestException("Only text messages can be edited.");
            }

            var resource = new ConversationResource(message.ConversationId, message.Id, message.SenderId);

            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser.Principal, resource, ConversationPermissions.EditMessage);

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException("You cannot edit this message.");
            }
            var newContent = dto.Content.Trim();

            if (message.Content == newContent)
            {
                return mapper.Map<MessageDto>(message);
            }

            var oldVersion = new MessageVersion
            {
                Id = Guid.NewGuid(),
                MessageId = message.Id,
                Content = message.Content ?? string.Empty,
                Version = message.Versions.Count, // Assuming message.Version is a collection of MessageVersion
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.MessageVersions.AddAsync(oldVersion);

            message.Content = newContent;
            message.Version++; 
          
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;

            var @event = new MessageEditedEvent
            (
                MessageId: message.Id,
                ConversationId: message.ConversationId,
                UserId: userId,
                Content: message.Content,
                Version:message.Version,
                EditedAt: message.EditedAt.Value
            );

            await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(@event));
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<MessageDto>(message);
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

        public Task<MessageDto> SendFileAsync(Guid senderId, SendFileDto filedto)
        {
           // validator.ValidateFile(filedto.)
           throw new NotImplementedException();
        }

        public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto dto,CancellationToken cancellationToken = default)
        {
            if (dto.ConversationId == Guid.Empty)
                throw new ArgumentException("ConversationId is required.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException("Message content cannot be empty.");

            var resource = new ConversationResource(dto.ConversationId);

            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser.Principal, resource, ConversationPermissions.SendMessage);

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException("you cann't send messages to this conversation.");
            }

            //valid reply
            Message? repliedMessage = null;


            if (dto.ReplyToMessageId.HasValue)
            {
                repliedMessage = await unitOfWork.Messages.GetByIdAsync(dto.ReplyToMessageId.Value);


                if (repliedMessage is null)
                {
                    throw new KeyNotFoundException("The message you are replying to was not found.");
                }


                // Very important:
                // Reply must belong to the same conversation.

                if (repliedMessage.ConversationId != dto.ConversationId)
                {
                    throw new InvalidOperationException("You cannot reply to a message from another conversation.");
                }
                if (repliedMessage.IsDeletedForEveryone)
                    throw new BadRequestException("you cann't reply to a deleted message.");
                var deletedForUser =await unitOfWork.Messages
                     .IsDeletedForUserAsync(repliedMessage.Id,senderId);

                if (deletedForUser)
                    throw new NotFoundException("The message you are replying to was not found.");
            }
            var sentAt =dto.SendAt ?? DateTime.UtcNow;

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = dto.ConversationId,
                SenderId = senderId,
                Content = dto.Content.Trim(),
                MessageType = MessageType.Text,
                ReplyToMessageId = repliedMessage?.Id,
                SendAt = sentAt
            };

            
            await unitOfWork.Messages.AddAsync(message);
        
           await messageStatusService.CreateRecipientsAsync(message);

            var @event =new MessageCreatedEvent(MessageId: message.Id,ConversationId: message.ConversationId,SenderId: senderId,
             Content: message.Content, MessageType: message.MessageType,ReplyToMessageId: message.ReplyToMessageId, SentAt: message.SendAt);

            await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(@event), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var senderName = await GetSenderNameAsync(senderId);


            ReplyPreviewDto? replyDto = null;
            if (repliedMessage is not null)
            {
                var repliedSenderName = await GetSenderNameAsync(repliedMessage.SenderId);

                replyDto = new ReplyPreviewDto
                {
                    Id = repliedMessage.Id,

                    SenderId = repliedMessage.SenderId,

                    SenderName = repliedSenderName,

                    Content = repliedMessage.IsDeletedForEveryone ? null : repliedMessage.Content,

                    MessageType = repliedMessage.MessageType,

                    IsDeletedForEveryone = repliedMessage.IsDeletedForEveryone
                };
            }

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
                ReplyTo=replyDto,
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
