using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
     public interface IChatService
    {
        Task<bool> IsMember(int chatId, string userId);
        Task<bool> IsBlocked(int chatId, string senderId);
        Task<Message> CreateMessage(int chatId, string senderId, string text);

        Task SetUserOnline(string userId, string connectionId);
        Task SetUserOffline(string connectionId);
    }
}
