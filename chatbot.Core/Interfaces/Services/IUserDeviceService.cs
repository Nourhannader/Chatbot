using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IUserDeviceService
    {
        Task RegisterDeviceAsync(string userId, string deviceToken, string devicaType);
        Task SetOnlineAsync(string userId, string deviceToken, string connectionId);
        Task SetOfflineAsync(string connectionId);
        Task<List<UserDevice>> GetDeviceAsync(string userId);

    }
}
