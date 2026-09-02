using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IReactionService
    {
        Task<MessageReaction?> AddReactionAsync(Guid messageId, Guid userId, ReactionType reactionType);
        Task RemoveReactionAsync(Guid messageId, Guid userId);
        Task<List<MessageReaction>> GetMessageReactionsAsync(Guid messageId);
    }
}
