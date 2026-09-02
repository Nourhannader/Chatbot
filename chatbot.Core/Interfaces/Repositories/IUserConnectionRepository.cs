using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IUserConnectionRepository:IBaseRepository<UserConnection,Guid>
    {
        Task RemoveAsync(Guid connectionId);

        Task<List<UserConnection>> GetUserConnectionsAsync(Guid userId);

        Task<bool> HasConnectionsAsync(Guid userId);
    }
}
