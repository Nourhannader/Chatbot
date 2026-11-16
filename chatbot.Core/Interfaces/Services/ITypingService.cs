using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface ITypingService
    {
        Task StartTypingAsync(Guid conversationId, Guid userId,string connectionId);

        Task<bool> StopTypingAsync(Guid conversationId, Guid userId, string connectionId);

        Task<List<(Guid ConversationId, Guid UserId)>> RemoveConnectionAsync(string connectionId);

        Task<bool> IsTypingAsync(Guid conversationId,Guid userId);

        Task<List<Guid>> GetTypingUsersAsync(Guid conversationId);
    }
}
