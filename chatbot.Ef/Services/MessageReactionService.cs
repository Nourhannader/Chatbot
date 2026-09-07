using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Google;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Services
{
    public class MessageReactionService(IUnitOfWork unitOfWork) : IMessageReactionService
    {
        public async Task<MessageReaction> AddReactionAsync(Guid messageId, Guid userId, ReactionType type)
        {
            if (type == ReactionType.None)
                throw new ArgumentException("Reaction type cannot be None", nameof(type));


            var message = await unitOfWork.Messages.GetByIdAsync(messageId);
            if (message == null)
                throw new KeyNotFoundException("Message not found");
            var reaction =  await unitOfWork.Reactions.GetReactionByMessageIdAndUserIdAsync(messageId, userId);
            if (reaction == null)
            {
                reaction = new MessageReaction
                {
                    MessageId = messageId,

                    UserId = userId,

                    ReactionType=type,

                    CreatedAt = DateTime.UtcNow
                };

               await unitOfWork.Reactions.AddAsync(reaction);
            }
            else
            {
                reaction.ReactionType=type;

                reaction.CreatedAt =
                    DateTime.UtcNow;
                unitOfWork.Reactions.Update(reaction);
            }
            await unitOfWork.SaveChangesAsync();

            return reaction;

        }

        public async Task RemoveReactionAsync(Guid messageId, Guid userId)
        {
            var reaction= await unitOfWork.Reactions.GetReactionByMessageIdAndUserIdAsync(messageId, userId);
            if (reaction == null)
                return;

            unitOfWork.Reactions.RemoveMessageReaction(reaction);
            await unitOfWork.SaveChangesAsync();

        }
    }
}
