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
        Task<bool> ConversationExistsAsync(Guid conversationId);
        Task<bool> IsMemberAsync(Guid conversationId, Guid memberId);
        Task AddRangeAsync(IEnumerable<Message> messages);
    }
}
