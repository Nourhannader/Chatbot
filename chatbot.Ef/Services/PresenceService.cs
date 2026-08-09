using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Ef.Services
{
    public class PresenceService
        (IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager) : IPresenceService
    {
        public async Task<List<string>> GetConnectionIdsAsync(string userId)
        {
            var connections=await  unitOfWork.UserConnections.GetUserConnectionsAsync(userId);

            return connections.Select(c => c.ConnectionId).ToList();
        }

        public async Task<DateTime?> GetLastSeenAsync(string userId)
        {
            var user=await userManager.FindByIdAsync(userId);
            return user?.LastSeen;
        }

        public async Task<bool> IsOnlineAsync(string userId)
        {
            return await unitOfWork.UserConnections.HasConnectionsAsync(userId);
        }


        public async Task UserConnectedAsync(string userId, string connectionId, string deviceType)
        {
            await unitOfWork.UserConnections.AddAsync(new UserConnection
            {
                UserId = userId,
                ConnectionId = connectionId,
                DeviceType = deviceType
            });
            await unitOfWork.SaveChangesAsync();
            
            var user=await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsOnline = true;
                user.LastSeen = DateTime.UtcNow;
                await userManager.UpdateAsync(user);
            }

        }

        public async Task UserDisconnectedAsync(string connectionId)
        {
            var connection= await unitOfWork.UserConnections.GetByIdAsync(connectionId);
            if(connection == null)
            {
                return;
            }
            var userId = connection.UserId;
            await unitOfWork.UserConnections.RemoveAsync(connectionId);
            await unitOfWork.SaveChangesAsync();

            var hasOtherConnections = await unitOfWork.UserConnections.HasConnectionsAsync(userId);
            if(hasOtherConnections)
            {
                return;
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsOnline = false;
                user.LastSeen = DateTime.UtcNow;
                await userManager.UpdateAsync(user);
            }
        }
    }
}
