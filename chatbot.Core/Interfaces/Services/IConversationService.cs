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
        Task<Conversation> CreateConversationAsync(Guid creatorId, Guid secondUserId);
        Task<Conversation> CreateGroupAsync(Guid creatorId, string title, List<string> members);
        Task<List<Conversation>> GetUserConversationsAsync(Guid userId);
        Task AddMemberAsync(Guid conversationId, Guid userId);
        Task RemoveMemberAsync(Guid conversationId, Guid userId);
    }
}
