using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class UserDeviceService(IUnitOfWork unitOfWork) : IUserDeviceService
    {
        public async Task<List<UserDevice>> GetUserDevicesAsync(Guid userId)
        {
           return await unitOfWork.UserDevices.GetActiveDevicesAsync(userId);
        }

        public async Task RegisterAsync(Guid userId, RegisterDeviceDto dto)
        {
            var device = await unitOfWork.UserDevices.GetAsync(userId, dto.PushToken,dto.Provider);
            if(device == null)
            {
                device = new UserDevice
                {
                    UserId = userId,
                    PushToken = dto.PushToken,
                    DeviceType = dto.DeviceType,
                    Provider=dto.Provider,
                    DeviceName = dto.DeviceName,
                    IsActive = true,
                    LastUsedAt = DateTime.UtcNow
                };
                await unitOfWork.UserDevices.AddAsync(device);
            }
            else
            {
                device.IsActive = true;
                device.DeviceType = dto.DeviceType;
                device.DeviceName = dto.DeviceName;
                device.LastUsedAt = DateTime.UtcNow;

                unitOfWork.UserDevices.Update(device);
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid userId,string pushToken,PushProvider provider)
        {
            await unitOfWork.UserDevices.DeactivateAsync(userId,pushToken, provider);

            await unitOfWork.SaveChangesAsync();
        }
    }
}
