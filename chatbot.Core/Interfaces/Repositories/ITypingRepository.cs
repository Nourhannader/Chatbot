using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface ITypingRepository
    {
        bool StartTyping( Guid conversationId, Guid userId, string connectionId);

        bool StopTyping(Guid conversationId,Guid userId, string connectionId);

        List<Guid> GetTypingUsers(Guid conversationId);

        bool IsTyping(Guid conversationId, Guid userId);

        List<(Guid ConversationId, Guid UserId)> RemoveConnection( string connectionId);
    }
}
