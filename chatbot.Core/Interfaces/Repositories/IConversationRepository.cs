using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IConversationRepository :IBaseRepository<Conversation,Guid>
    {
        void Delete(Conversation conversation);
        
        Task<Conversation?> GetGroupWithMembersAsync(Guid ConversationId);
        
        Task<Conversation?> GetWithImageAsync(Guid conversationId);

        Task<Conversation?> GetDetailsAsync(Guid conversationId);

        Task<Conversation?> GetPrivateAsync(Guid firstUserId, Guid secondUserId);

        Task<List<Conversation>> GetUserConversationsAsync(Guid userId);

        Task<Conversation?> GetPrivateByKeyAsync(string privateKey);

        Task<bool> ExistsAsync(Guid conversationId);

        Task<bool> ConversationExistsAsync(Guid firstUserId, Guid secondUserId);


    }
}
