using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IPresenceService
    {
        Task UserConnectedAsync(Guid userId,string connectionId);
        Task UserDisconnectedAsync(Guid userId, string connectionId);
        Task<bool> IsOnlineAsync(Guid userId);
        Task<int> GetConnectionCountAsync(Guid userId);
    }
}
