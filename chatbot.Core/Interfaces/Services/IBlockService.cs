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
        Task BlockAsync(Guid blockerId, Guid blockedId);

        Task UnblockAsync(Guid blockerId, Guid blockedId);

        Task<bool> IsBlockedAsync(Guid firstUserId, Guid secondUserId);

        Task<List<BlockList>> GetBlockedUsersAsync(Guid blockerId);
    }
}
