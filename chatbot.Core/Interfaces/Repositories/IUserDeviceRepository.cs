using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IUserDeviceRepository:IBaseRepository<UserDevice,Guid>
    {
        Task<UserDevice?> GetByTokenAsync(
        string deviceToken);

        Task<IEnumerable<UserDevice>>
            GetUserDevicesAsync(
                Guid userId);

        Task<IEnumerable<UserDevice>>
            GetActiveDevicesAsync(
                Guid userId);

        

    }
}
