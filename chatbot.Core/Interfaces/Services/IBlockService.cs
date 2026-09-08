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
        Task BlockAsync(Guid blockerId, Guid blockedUserId);

        Task UnblockAsync(Guid blockerId, Guid blockedUserId);

        Task<bool> IsBlockedAsync(Guid userId, Guid otherUserId);

        Task<List<Guid>> GetBlockedUsersAsync(Guid userId);
    }
}
