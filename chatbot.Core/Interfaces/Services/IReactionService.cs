using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IReactionService
    {
        Task<MessageReaction?> AddReactionAsync(string messageId, string userId, ReactionType reactionType);
        Task RemoveReactionAsync(string messageId, string userId);
        Task<List<MessageReaction>> GetMessageReactionsAsync(string messageId);
    }
}
