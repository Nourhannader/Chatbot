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
        Task<Conversation> CreateConversationAsync(string creatorId, string secondUserId);
        Task<Conversation> CreateGroupAsync(string creatorId, string title, List<string> members);
        Task<List<Conversation>> GetUserConversationsAsync(string userId);
        Task AddMemberAsync(string conversationId, string userId);
        Task RemoveMemberAsync(string conversationId, string userId);
    }
}
