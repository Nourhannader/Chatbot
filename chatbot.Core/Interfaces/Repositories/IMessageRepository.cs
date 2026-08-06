using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IMessageRepository : IBaseRepository<Message,string>
    {
        Task<List<Message>> GetConversationMessagesAsync(
        string conversationId,
        int page,
        int pageSize);
    }
}
