using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IConversationMemberRepository:IBaseRepository<ConversationMember,Guid>
    {
        Task<ConversationMember?> GetAsync(Guid conversationId,Guid userId);

        Task<List<ConversationMember>> GetActiveMembersAsync(Guid conversationId);

        Task<bool> ExistsAsync(Guid conversationId,Guid userId);
        void Remove(ConversationMember member);
    }
}
