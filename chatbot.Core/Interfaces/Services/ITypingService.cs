using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface ITypingService
    {
        Task StartTypingAsync(Guid conversationId, Guid userId);
        Task StopTypingAsync(Guid conversationId,Guid userId);
        Task<bool> IsTypingAsync(Guid conversationId, Guid userId);
        Task<List<string>> GetTypingUsersAsync(Guid conversationId);
    }
}
