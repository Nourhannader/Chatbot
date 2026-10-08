using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Reactions;
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

            if (message is null || message.IsDeleted)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var isMember = await unitOfWork.ConversationMember
                .IsMemberAsync(message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            var reactions = await unitOfWork.Reactions.GetByMessageIdAsync(messageId);

            return reactions.Select(r => new MessageReactionDto
            {
                Id = r.Id,
                MessageId = r.MessageId,
                UserId = r.UserId,
                ReactionType = r.ReactionType,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<ReactionResultDto> ToggleReactionAsync(Guid userId, AddReactionDto dto)
        {
            // 1. Validate the reaction type.
            if (!Enum.IsDefined(typeof(ReactionType), dto.ReactionType))
            {
                throw new ArgumentException("Invalid reaction type.");
            }

            // 2. Get the message.
            var message = await unitOfWork.Messages.GetByIdAsync(dto.MessageId);

            if (message is null || message.IsDeleted)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            // 3. Check conversation membership.
            var isMember = await unitOfWork.ConversationMember.IsMemberAsync(
                    message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            // 4. Get the user's existing reaction.
            var existing = await unitOfWork.Reactions
                .GetByMessageAndUserAsync(dto.MessageId, userId);

            // 5. If the same reaction exists, remove it.
            if (existing is not null && existing.ReactionType == dto.ReactionType)
            {
               unitOfWork.Reactions.Remove(existing);

                await unitOfWork.SaveChangesAsync();

                return new ReactionResultDto
                {
                    MessageId = dto.MessageId,
                    UserId = userId,
                    ReactionType = null,
                    IsRemoved = true
                };
            }

            // 6. If another reaction exists, update it.
            if (existing is not null)
            {
                existing.ReactionType = dto.ReactionType;

                unitOfWork.Reactions.Update(existing);

                await unitOfWork.SaveChangesAsync();

                return new ReactionResultDto
                {
                    MessageId = dto.MessageId,
                    UserId = userId,
                    ReactionType = existing.ReactionType,
                    IsRemoved = false
                };
            }

            // 7. Create a new reaction.
            var reaction = new MessageReaction
            {
                MessageId = dto.MessageId,
                UserId = userId,
                ReactionType = dto.ReactionType,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Reactions.AddAsync(reaction);

            await unitOfWork.SaveChangesAsync();

            return new ReactionResultDto
            {
                MessageId = dto.MessageId,
                UserId = userId,
                ReactionType = reaction.ReactionType,
                IsRemoved = false
            };
        }
    }
}
