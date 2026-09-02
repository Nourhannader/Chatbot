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

        public async Task<BlockList?> GetAsync(Guid blockerId, Guid blockedId)
        {
            return await context.BlockLists
                .FirstOrDefaultAsync(b =>
                b.BlockerId == blockerId && b.BlockedId == blockedId
                );
        }

        public async Task<List<BlockList>> GetBlockedUsersAsync(Guid blockerId)
        {
            return await context.BlockLists
                .Where(b => b.BlockerId == blockerId)
                .Include(b => b.Blocked)
                .ToListAsync();
        }

        public async Task<BlockList?> GetByIdAsync(Guid id)
        {
           return await context.BlockLists.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> IsBlockedAsync(Guid firstUserId, Guid secondUserId)
        {
           return await context.BlockLists.AnyAsync(b =>
                      (b.BlockerId==firstUserId && b.BlockedId == secondUserId)||
                      (b.BlockerId == secondUserId && b.BlockedId == firstUserId));
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
