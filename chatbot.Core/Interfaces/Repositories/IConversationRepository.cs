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
        Task<List<Conversation>> GetUserConversationsAsync(Guid userId);
        Task<bool> ConversationExistsAsync(Guid firstUserId, Guid secondUserId);
        Task<bool> IsMemberAsync(Guid conversationId,Guid userId,CancellationToken cancellationToken = default);

    }
}
