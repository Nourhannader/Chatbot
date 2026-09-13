using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IUserDeviceService
    {
        Task RegisterAsync(Guid userId, RegisterDeviceDto dto);

        Task RemoveAsync(Guid userId, string pushToken, PushProvider provider);

        Task<List<UserDevice>>GetUserDevicesAsync( Guid userId);

    }
}
