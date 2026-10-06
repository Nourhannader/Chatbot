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
        
        Task<UserConnection?> GetByConnectionIdAsync(string connectionId);

        Task<IEnumerable<UserConnection>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<UserConnection>> GetActiveConnectionsAsync(Guid userId);

        Task<bool> HasActiveConnectionAsync(Guid userId);

        Task<int> GetActiveConnectionCountAsync(Guid userId);

        void Delete(UserConnection connection);
        Task RemoveAsync(string connectionId);
    }
}
