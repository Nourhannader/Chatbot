using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.UserConnection;
using chatbot.Core.Enums;
using chatbot.Core.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace chatbot.Core.Interfaces.Services
{
    public interface IUserConnectionService
    {
        Task<bool> IsOnlineAsync(Guid userId);

        Task AddConnectionAsync(Guid userId, string connectionId, Guid? userDeviceId);

        Task RemoveConnectionAsync(string connectionId);

        Task<UserConnectionDto?> GetByConnectionIdAsync(string connectionId);
        public Task<UserConnectionStatusDto> GetOnlineStatusAsync(Guid userId);
    }
}
