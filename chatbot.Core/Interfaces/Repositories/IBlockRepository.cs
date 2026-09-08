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
        Task<BlockList?> GetAsync(Guid blockerId, Guid blockedUserId);
        Task<bool> IsBlockedAsync(Guid userId, Guid otherUserId);
        Task<bool> CheckFoundOrNot(Guid userId, Guid blockedUserId);    
        Task<List<Guid>> GetBlockedUsersAsync(Guid blockerId);
        Task RemoveAsync(BlockList block);
    }
}
