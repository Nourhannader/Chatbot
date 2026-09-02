using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class UserDeviceService(IUnitOfWork unitOfWork) : IUserDeviceService
    {
        public Task<List<UserDevice>> GetDeviceAsync(Guid userId)
        {
            return unitOfWork.UserDevices.GetUserDevicesAsync(userId);
        }

        public async Task RegisterDeviceAsync(Guid userId, string deviceToken, string devicaType)
        {
            var device=await unitOfWork.UserDevices.GetByUserAsync(userId, deviceToken);
            if (device==null)
            {
                return;
            }
            await unitOfWork.UserDevices.AddAsync(new UserDevice
            {
                UserId=userId,
                DeviceToken=deviceToken,
                DeviceType = devicaType,
            });
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SetOfflineAsync(string connectionId)
        {
            var devices = await unitOfWork.UserDevices.GetUserDevicesAsync(Guid.Empty); 
            var device = devices.FirstOrDefault(d => d.ConnectionId == connectionId);
            if (device == null)
            {
                return;
            }
            device.IsOnline = false;
            device.ConnectionId = null;
            device.DisconnectedAt = DateTime.UtcNow;
            unitOfWork.UserDevices.Update(device);
            await unitOfWork.SaveChangesAsync();
        }


        public async Task SetOnlineAsync(Guid userId, string deviceToken, string connectionId)
        {
            var device=await unitOfWork.UserDevices.GetByUserAsync(userId, deviceToken);
            if(device== null)
            {
                return;
            }
            device.ConnectionId = connectionId;
            device.IsOnline = true;
            device.ConnectedAt = DateTime.UtcNow;
            unitOfWork.UserDevices.Update(device);
            await unitOfWork.SaveChangesAsync();
        }

        
    }
}
