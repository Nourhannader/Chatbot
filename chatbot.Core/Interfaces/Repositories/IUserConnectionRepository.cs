using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IUserConnectionRepository:IBaseRepository<UserConnection,string>
    {
        Task RemoveAsync(string connectionId);

        Task<List<UserConnection>> GetUserConnectionsAsync(string userId);

        Task<bool> HasConnectionsAsync(string userId);
    }
}
