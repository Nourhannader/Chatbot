using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IPresenceService
    {
        Task UserConnectedAsync(string userId,string connectionId,string deviceType);
        Task UserDisconnectedAsync( string connectionId);
        Task<bool> IsOnlineAsync(string userId);
        Task<DateTime?> GetLastSeenAsync(string userId);

        Task<List<string>> GetConnectionIdsAsync(string userId);
    }
}
