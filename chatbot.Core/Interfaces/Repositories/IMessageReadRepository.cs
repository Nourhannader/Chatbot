using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IMessageReadRepository:IBaseRepository<MessageRead,Guid>
    {

        Task<bool> ExistsAsync(Guid messageId, Guid userId);

        Task<MessageRead?> GetByMessageAndUserAsync(Guid messageId, Guid userId);

        Task<IEnumerable<MessageRead>> GetByMessageIdAsync(Guid messageId);

        Task AddRangeAsync(IEnumerable<MessageRead> messageReads);
    }
}
