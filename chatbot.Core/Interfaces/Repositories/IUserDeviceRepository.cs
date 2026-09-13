using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IUserDeviceRepository:IBaseRepository<UserDevice,Guid>
    {

        Task<IEnumerable<UserDevice>>GetUserDevicesAsync(Guid userId);

        Task<List<UserDevice>>GetActiveDevicesAsync(Guid userId);

        Task DeactivateAsync(Guid userId, string pushToken, PushProvider provider);
        Task<UserDevice?> GetAsync(Guid userId,string pushToken,PushProvider provider);

    }
}
