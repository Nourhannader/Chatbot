using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public sealed class UserDeviceService(IUnitOfWork unitOfWork,IMapper mapper) : IUserDeviceService
    {
        public async Task<List<UserDeviceDto>> GetUserDevicesAsync(Guid userId)
        {
           var devices= await unitOfWork.UserDevices.GetActiveDevicesAsync(userId);
            return mapper.Map<List<UserDeviceDto>>(devices);
        }

        public async Task RegisterAsync(Guid userId, RegisterDeviceDto dto)
        {
            var device = await unitOfWork.UserDevices.GetByPushTokenAsync( dto.PushToken,dto.Provider);
            if(device == null)
            {
                device = new UserDevice
                {
                    Id=Guid.NewGuid(),
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
                device.UserId = userId;
                device.DeviceType = dto.DeviceType;
                device.DeviceName = dto.DeviceName;
                device.IsActive = true;
                device.LastUsedAt = DateTime.UtcNow;
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
