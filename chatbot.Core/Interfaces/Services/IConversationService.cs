using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IConversationService
    {
     
        //private conversation
        Task<Conversation> CreateConversationAsync(Guid creatorId, Guid secondUserId);
        Task<List<Conversation>> GetUserConversationsAsync(Guid userId);
    }
}
