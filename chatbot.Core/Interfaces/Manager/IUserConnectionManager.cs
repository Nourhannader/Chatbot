using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Manager
{
    public interface IUserConnectionManager
    {
        Task AddConnectionAsync(
            Guid userId,
            string connectionId);

        Task RemoveConnectionAsync(
            Guid userId,
            string connectionId);

        Task<IReadOnlyCollection<string>>
            GetConnectionsAsync(Guid userId);
    }
}
