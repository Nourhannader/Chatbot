using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class ReactionService(IUnitOfWork unitOfWork) : IReactionService
    {
        public async Task<MessageReaction?> AddReactionAsync(string messageId, string userId, ReactionType reactionType)
        {
            var message=await unitOfWork.Messages.GetByIdAsync(messageId);
            if (message == null)
            {
                throw new Exception("Message not found");
            }

            var existingReaction=await unitOfWork.Reactions.GetReactionByMessageIdAndUserIdAsync(messageId, userId);
            if(existingReaction != null)
            {
                existingReaction.ReactionType = reactionType;
                existingReaction.CreatedAt = DateTime.UtcNow;
                unitOfWork.Reactions.Update(existingReaction);
                await unitOfWork.SaveChangesAsync();
                return existingReaction;
            }
            var reaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                ReactionType = reactionType,
                CreatedAt = DateTime.UtcNow
            };
            await unitOfWork.Reactions.AddAsync(reaction);
            await unitOfWork.SaveChangesAsync();
            return reaction;
        }

        public async Task<List<MessageReaction>> GetMessageReactionsAsync(string messageId)
        {
            return await unitOfWork.Reactions.GetMessageReactionsAsync(messageId);
        }

        public async Task RemoveReactionAsync(string messageId, string userId)
        {
            var reaction = await unitOfWork.Reactions.GetReactionByMessageIdAndUserIdAsync(messageId, userId);
            if (reaction == null)
            {
                throw new Exception("Reaction not found");
            }
            await unitOfWork.Reactions.RemoveMessageReaction(reaction);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
