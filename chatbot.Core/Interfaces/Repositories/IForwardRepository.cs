using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IForwardRepository
    {
        Task<bool> ConversationExistsAsync(string conversationId);
        Task<bool> IsMemberAsync(string conversationId,string memberId);
        Task AddRangeAsync(IEnumerable<Message> messages);
    }
}
