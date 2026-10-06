using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.User;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Ef.Services
{
    public class PresenceService(IUnitOfWork unitOfWork) : IPresenceService
    {
        private readonly ConcurrentDictionary<string,ConcurrentDictionary<string, byte>>users = new();

        public async Task<int> GetConnectionCountAsync(Guid userId)
        {
            return await unitOfWork.UserConnections.GetActiveConnectionCountAsync(userId);
        }

        public async Task<UserPresenceDto> GetPresenceAsync(Guid userId)
        {
            var isOline= await unitOfWork.UserConnections.HasActiveConnectionAsync(userId);

            var user = await unitOfWork.Auth.GetByIdAsync(userId);
            

            var connectionCount = await unitOfWork.UserConnections.GetActiveConnectionCountAsync(userId);

            return new UserPresenceDto
            {
                UserId = userId,
                IsOnline = isOline,
                ConnectionCount = connectionCount,
                LastSeenAt = user?.LastSeenAt
            };
        }

        public  async Task UserDisconnectedAsync(Guid userId)
        {
            var isOnline = await unitOfWork.UserConnections.HasActiveConnectionAsync(userId);
                 

            // Still connected from another device
            if (isOnline)
                return;

            var user = await unitOfWork.Auth.GetByIdAsync(userId);

            if (user == null)
                return;

            user.LastSeenAt =DateTime.UtcNow;

            await unitOfWork.Auth.updateState(user);
        }
    }
}
