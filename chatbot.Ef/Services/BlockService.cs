using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class BlockService(IUnitOfWork unitOfWork) : IBlockService
    {
        public async Task BlockAsync(Guid blockerId, Guid blockedUserId)
        {
            if (blockerId == blockedUserId)
                throw new InvalidOperationException("You cannot block yourself.");
            var exists=await unitOfWork.Blocks.CheckFoundOrNot(blockerId,blockedUserId);
            if (exists)
                return;

            await unitOfWork.Blocks.AddAsync(new BlockList
            {
                BlockerId = blockerId,
                BlockedUserId = blockedUserId,
                BlockedAt = DateTime.UtcNow
            });
            await unitOfWork.SaveChangesAsync();

        }

        public async Task<List<Guid>> GetBlockedUsersAsync(Guid userId)
        {
            return await unitOfWork.Blocks.GetBlockedUsersAsync(userId);
        }

        public async Task<bool> IsBlockedAsync(Guid userId, Guid otherUserId)
        {
            return await unitOfWork.Blocks.IsBlockedAsync(userId, otherUserId);
        }

        public async Task UnblockAsync(Guid blockerId, Guid blockedUserId)
        {
            var block =await unitOfWork.Blocks.GetAsync(blockerId, blockedUserId);
            if (block == null)
                return;
            await unitOfWork.Blocks.RemoveAsync(block);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
