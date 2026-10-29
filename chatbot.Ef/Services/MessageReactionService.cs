using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using chatbot.Core.Events.Messages;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Outbox;
using chatbot.Ef.Repositories;
using chatbot.Ef.UnitOfWork;
using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Services
{
    public class MessageReactionService(IUnitOfWork unitOfWork,IMapper mapper,IAuthorizationService authorizationService,ICurrentUser currentUser,
        OutboxMessageFactory outboxMessageFactory) : IMessageReactionService
    {
        public async Task<List<MessageReactionDto>> GetByMessageIdAsync(Guid userId, Guid messageId)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message is null || message.IsDeletedForEveryone)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var resource = new ConversationResource(message.ConversationId, messageId);
            
            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser.Principal, resource, ConversationPermissions.View);

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException( "You cannot view reactions for this message.");
            }

            var deletedForUser =await unitOfWork.Messages.IsDeletedForUserAsync(messageId, userId);

            if (deletedForUser)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var reactions = await unitOfWork.Reactions.GetByMessageIdAsync(messageId);

            return reactions
             .Select(r => new MessageReactionDto
                 {
                     Id = r.Id,
                     MessageId = r.MessageId,
                     UserId = r.UserId,
                     UserName = r.User.UserName ?? string.Empty,
                     ReactionType = r.ReactionType,
                     CreatedAt = r.CreatedAt,
                     UpdatedAt = r.UpdatedAt
                 })
             .ToList();
        }

        public async Task<MessageReactionDto> ToggleReactionAsync(Guid userId, AddReactionDto dto)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(dto.MessageId);

            if (message is null)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var resource = new ConversationResource(message.ConversationId, messageId: dto.MessageId);

            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser.Principal, resource, ConversationPermissions.React);

            if (!authorizationResult.Succeeded)
                throw new ForbiddenException("you can't react to this message");

            var deletedForUser = await unitOfWork.Messages.IsDeletedForUserAsync(message.Id, userId);

            if (deletedForUser)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var existing = await unitOfWork.Reactions
                 .GetByMessageAndUserAsync(dto.MessageId, userId);

            var now = DateTime.UtcNow;

            // =====================================
            // Same reaction => REMOVE
            // =====================================

            if (existing is not null && existing.ReactionType == dto.ReactionType)
            {
                var removedReactionType =existing.ReactionType;

                var result = new MessageReactionDto
                {
                    Id = existing.Id,
                    MessageId = existing.MessageId,
                    UserId = existing.UserId,
                    ReactionType = removedReactionType,
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = existing.UpdatedAt
                };

                unitOfWork.Reactions.Remove(existing);
                var @event = new MessageReactedEvent(MessageId: message.Id, ConversationId: message.ConversationId,
                    UserId: userId,ReactionType: removedReactionType, IsRemoved: true, ReactedAt: now);
              
                await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(@event));

                await unitOfWork.SaveChangesAsync();

                return result;
            }
            // =====================================
            // Different reaction => UPDATE
            // =====================================

            if (existing is not null)
            {
                existing.ReactionType = dto.ReactionType;
                existing.UpdatedAt = DateTime.UtcNow;
                // No Update() needed because
                // existing entity is tracked.

                var @event = new MessageReactedEvent(MessageId: message.Id, ConversationId: message.ConversationId,
                    UserId: userId, ReactionType: dto.ReactionType, IsRemoved: false, ReactedAt: now);

                await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(@event));

                await unitOfWork.SaveChangesAsync();
                return mapper.Map<MessageReactionDto>(existing);
            }
            // =====================================
            // No reaction => ADD
            // =====================================
            
            
              var reaction = new MessageReaction
                  {
                    Id = Guid.NewGuid(),
                    MessageId = message.Id,
                    UserId = userId,
                    ReactionType = dto.ReactionType,
                    CreatedAt = now
                  };
             await unitOfWork.Reactions.AddAsync(reaction);

            var addedEvent = new MessageReactedEvent(MessageId: message.Id,ConversationId: message.ConversationId,
                UserId: userId,ReactionType:dto.ReactionType,IsRemoved: false, ReactedAt: now);
            await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(addedEvent));

            await unitOfWork.SaveChangesAsync();

            return mapper.Map<MessageReactionDto>(reaction); 
        }

        
    }
}
