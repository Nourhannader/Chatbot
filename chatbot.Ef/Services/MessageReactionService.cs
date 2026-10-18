using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Repositories;
using chatbot.Ef.UnitOfWork;
using Google;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Services
{
    public class MessageReactionService(IUnitOfWork unitOfWork) : IMessageReactionService
    {
        public async Task<List<MessageReactionDto>> GetByMessageIdAsync(Guid userId, Guid messageId)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message is null || message.IsDeletedForEveryone)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var isMember = await unitOfWork.ConversationMember
                .ExistsAsync(message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            var deletedForUser =await unitOfWork.Messages
                .IsDeletedForUserAsync(messageId, userId);

            if (deletedForUser)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var reactions = await unitOfWork.Reactions.GetByMessageIdAsync(messageId);

            return reactions
             .Select(r =>
                 new MessageReactionDto
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

        public async Task<ReactionResultDto> ToggleReactionAsync(Guid userId, AddReactionDto dto)
        {
            var message = await unitOfWork.Messages
                .GetByIdAsync(dto.MessageId);

            if (message is null || message.IsDeletedForEveryone)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var isMember = await unitOfWork.ConversationMember.ExistsAsync(
                    message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            var deletedForUser = await unitOfWork.Messages
                .IsDeletedForUserAsync(message.Id, userId);

            if (deletedForUser)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var existing = await unitOfWork.Reactions
                 .GetByMessageAndUserAsync(dto.MessageId, userId);

            // =====================================
            // Same reaction => REMOVE
            // =====================================

            if (existing is not null && existing.ReactionType == dto.ReactionType)
            {
                unitOfWork.Reactions.Remove(existing);
                await unitOfWork.SaveChangesAsync();

                return new ReactionResultDto
                {
                    MessageId = message.Id,
                    ConversationId = message.ConversationId,
                    UserId = userId,
                    ReactionType = null,
                    Action = ReactionAction.Removed
                };
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
                await unitOfWork.SaveChangesAsync();
                return new ReactionResultDto
                {
                    MessageId = message.Id,
                    ConversationId = message.ConversationId,
                    UserId = userId,
                    ReactionType = existing.ReactionType,
                    Action = ReactionAction.Updated
                };
            }
            // =====================================
            // No reaction => ADD
            // =====================================

            var reaction =
               new MessageReaction
               {
                   Id = Guid.NewGuid(),
                   MessageId = message.Id,
                   UserId = userId,
                   ReactionType = dto.ReactionType,
                   CreatedAt = DateTime.UtcNow
               };

            await unitOfWork.Reactions.AddAsync(reaction);
            await unitOfWork.SaveChangesAsync();
            return new ReactionResultDto
            {
                MessageId = message.Id,
                ConversationId = message.ConversationId,
                UserId = userId,
                ReactionType = reaction.ReactionType,
                Action = ReactionAction.Added
            };
        
    }
    }
}
