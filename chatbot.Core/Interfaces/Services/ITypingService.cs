using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface ITypingService
    {
        Task StartTypingAsync(string conversationId, string userId);
        Task StopTypingAsync(string conversationId,string userId);
        Task<bool> IsTypingAsync(string conversationId, string userId);
        Task<List<string>> GetTypingUsersAsync(string conversationId);
    }
}
