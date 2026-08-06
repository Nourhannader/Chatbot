using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IReactionRepository :IBaseRepository<MessageReaction,int>
    {
        Task<MessageReaction?> GetReactionByMessageIdAndUserIdAsync(string messageId, string userId);
        Task<List<MessageReaction>> GetMessageReactionsAsync(string messageId);
        Task RemoveMessageReaction(MessageReaction reaction);
    }
}
