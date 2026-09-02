using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IPresenceService
    {
        Task UserConnectedAsync(Guid userId,Guid connectionId,string deviceType);
        Task UserDisconnectedAsync( Guid connectionId);
        Task<bool> IsOnlineAsync(Guid userId);
        Task<DateTime?> GetLastSeenAsync(Guid userId);

        Task<List<string>> GetConnectionIdsAsync(Guid userId);
    }
}
