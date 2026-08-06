using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IBlockRepository:IBaseRepository<BlockList,string>
    {
        Task<BlockList?> GetAsync(string blockerId, string blockedId);
        Task<bool> IsBlockedAsync(string firstUserId, string secondUserId);
        Task<List<BlockList>> GetBlockedUsersAsync(string blockerId);
        Task RemoveAsync(BlockList block);
    }
}
