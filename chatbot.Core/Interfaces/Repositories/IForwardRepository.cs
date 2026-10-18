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
        Task AddRangeAsync(IEnumerable<Message> messages);
        Task<List<Guid>>GetValidConversationIdsAsync(Guid userId,IEnumerable<Guid> conversationIds);
    }
}
