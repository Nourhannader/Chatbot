using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class BlockRepository(ApplicationDbContext context) : IBlockRepository
    {
        public async Task AddAsync(BlockList entity)
        {
            await context.BlockLists.AddAsync(entity);
        }

        public async Task<bool> CheckFoundOrNot(Guid userId, Guid blockedUserId)
        {
            
            return await context.BlockLists
                .AnyAsync(x => 
                x.BlockerId == userId && 
                x.BlockedUserId==blockedUserId
                );
        }

        public async Task<BlockList?> GetAsync(Guid blockerId, Guid blockedUserId)
        {
            return await context.BlockLists
                .FirstOrDefaultAsync(b =>
                b.BlockerId == blockerId && b.BlockedUserId == blockedUserId
                );
        }

        public async Task<List<Guid>> GetBlockedUsersAsync(Guid blockerId)
        {
            return await context.BlockLists
                .Where(b => b.BlockerId == blockerId)
                .Select(b => b.BlockedUserId)
                .ToListAsync();
        }

        public async Task<BlockList?> GetByIdAsync(Guid id)
        {
           return await context.BlockLists.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> IsBlockedAsync(Guid userId, Guid otherUserId)
        {
           return await context.BlockLists.AnyAsync(b =>
                      (b.BlockerId==userId && b.BlockedUserId == otherUserId)||
                      (b.BlockerId == otherUserId && b.BlockedUserId == userId));
        }

        public Task RemoveAsync(BlockList block)
        {
            context.BlockLists.Remove(block);
            return Task.CompletedTask;
        }

        public void Update(BlockList entity)
        {
            context.BlockLists.Update(entity);
        }
    }
}
