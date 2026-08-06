using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IBlockService
    {
        Task BlockAsync(string blockerId, string blockedId);

        Task UnblockAsync(string blockerId, string blockedId);

        Task<bool> IsBlockedAsync(string firstUserId, string secondUserId);

        Task<List<BlockList>> GetBlockedUsersAsync(string blockerId);
    }
}
