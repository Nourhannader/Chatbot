using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IBlockRepository:IBaseRepository<BlockList,Guid>
    {
        Task<BlockList?> GetAsync(Guid blockerId, Guid blockedId);
        Task<bool> IsBlockedAsync(Guid firstUserId, Guid secondUserId);
        Task<List<BlockList>> GetBlockedUsersAsync(Guid blockerId);
        Task RemoveAsync(BlockList block);
    }
}
