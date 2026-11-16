using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.User;

namespace chatbot.Core.Interfaces.Services
{
    public interface IPresenceService
    {

        Task<int> GetConnectionCountAsync(Guid userId);

        Task<UserPresenceDto> GetPresenceAsync(Guid userId);

        Task UserConnectedAsync(Guid userId);

        Task UserDisconnectedAsync(Guid userId);
    }
}
