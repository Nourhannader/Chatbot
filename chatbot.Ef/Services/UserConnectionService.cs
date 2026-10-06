using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.UserConnection;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class UserConnectionService(IUnitOfWork unitOfWork,IMapper mapper) : IUserConnectionService
    {
        public async Task<bool> IsOnlineAsync(Guid userId)
        {
            return await unitOfWork.UserConnections.HasActiveConnectionAsync(userId);
        }

        public async Task AddConnectionAsync(Guid userId, string connectionId,Guid? userDeviceId)
        {
            var connection = new UserConnection
            {
                UserId = userId,

                UserDeviceId = userDeviceId,

                ConnectionId = connectionId,

                IsOnline = true,

                ConnectedAt = DateTime.UtcNow,

                DisconnectedAt = null
            };
            await unitOfWork.UserConnections.AddAsync(connection);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveConnectionAsync(string connectionId)
        {
            await unitOfWork.UserConnections.RemoveAsync(connectionId);
        }

        public async Task<UserConnectionDto?> GetByConnectionIdAsync(string connectionId)
        {
            var connection = await unitOfWork.UserConnections
                .GetByConnectionIdAsync(connectionId);
            if (connection == null)
                return null;
            return mapper.Map<UserConnectionDto>(connection);
        }
        
        public async Task<UserConnectionStatusDto> GetOnlineStatusAsync(Guid userId)
        {
            var count = await unitOfWork.UserConnections
               .GetActiveConnectionCountAsync(userId);

            return new UserConnectionStatusDto
            {
                UserId = userId,

                IsOnline = count > 0,

                ActiveConnections = count
            };
        }



    }
}
