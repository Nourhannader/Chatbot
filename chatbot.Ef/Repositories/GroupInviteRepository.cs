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
    public sealed class GroupInviteRepository(ApplicationDbContext context) : IGroupInviteRepository
    {
        public async Task AddAsync(GroupInvite entity)
        {
            await context.GroupInvites.AddAsync(entity);
        }

        public async Task<List<GroupInvite>> GetActiveInviteAsync(Guid conversationId,DateTime now)
        {
            return await context.GroupInvites
             .Where(x =>
                 x.ConversationId ==
                     conversationId &&
                 x.IsActive &&
                 x.ExpiresAt > now &&
                 x.UsedCount < x.MaxUses)
             .AsNoTracking()
             .ToListAsync();
        }

        public async Task<GroupInvite?> GetByCodeAsync(string code)
        {
            return await context.GroupInvites
            .Include(x => x.Conversation)
                .ThenInclude(x => x.Image)
            .FirstOrDefaultAsync(x =>
                x.Code == code);
        }

        public async Task<GroupInvite?> GetByIdAsync(Guid id)
        {
            return await context.GroupInvites.FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(GroupInvite entity)
        {
            context.GroupInvites.Update(entity);
        }
    }
}
