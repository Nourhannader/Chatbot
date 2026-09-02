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
        public async Task BlockAsync(Guid blockerId, Guid blockedId)
        {
            if (blockerId == blockedId)
                throw new Exception("You cannot block yourself.");
            var exists=await unitOfWork.Blocks.GetAsync(blockerId, blockedId);
            if (exists != null)
                return;

            await unitOfWork.Blocks.AddAsync(new BlockList
            {
                BlockerId = blockerId,
                BlockedId = blockedId,
                BlockedAt = DateTime.UtcNow
            });
            await unitOfWork.SaveChangesAsync();

        }

        public async Task<List<BlockList>> GetBlockedUsersAsync(Guid blockerId)
        {
            return await unitOfWork.Blocks.GetBlockedUsersAsync(blockerId);
        }

        public async Task<bool> IsBlockedAsync(Guid firstUserId, Guid secondUserId)
        {
            return await unitOfWork.Blocks.IsBlockedAsync(firstUserId, secondUserId);
        }

        public async Task UnblockAsync(Guid blockerId, Guid blockedId)
        {
            var block =await unitOfWork.Blocks.GetAsync(blockerId, blockedId);
            if (block == null)
                return;
            await unitOfWork.Blocks.RemoveAsync(block);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
