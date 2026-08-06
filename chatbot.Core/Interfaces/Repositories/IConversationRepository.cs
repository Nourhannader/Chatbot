using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IConversationRepository :IBaseRepository<Conversation,string>
    {
        Task<List<Conversation>> GetUserConversationsAsync(string userId);
        Task<bool> ConversationExistsAsync(string firstUserId, string secondUserId);

    }
}
