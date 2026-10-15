using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Core.Enums;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public sealed class ConversationMemberRepository(ApplicationDbContext context) : IConversationMemberRepository
    {
        public async Task AddAsync(ConversationMember entity)
        {
            await context.ConversationMembers.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<ConversationMember> members)
        {
             await context.ConversationMembers.AddRangeAsync(members);
        }

        public async Task<int> CountActiveAsync(Guid conversationId)
        {
            return await context.ConversationMembers
            .CountAsync(x =>
                x.ConversationId == conversationId &&
                x.LeftAt == null &&
                !x.IsBanned);
        }

        public async Task<bool> ExistsAsync(Guid conversationId, Guid userId)
        {
          return await context.ConversationMembers
                .AnyAsync(x => x.ConversationId == conversationId && x.UserId == userId && x.LeftAt ==null);   
        }

        public async Task<ConversationMember?> GetActiveAsync(Guid conversationId, Guid userId)
        {
            return await context.ConversationMembers
             .Include(x => x.User)
             .FirstOrDefaultAsync(x =>
                 x.ConversationId == conversationId &&
                 x.UserId == userId &&
                 x.LeftAt == null &&
                 !x.IsBanned);
        }

        public async Task<List<Guid>> GetActiveMemberIdsAsync(Guid conversationId)
        {
            return await context.ConversationMembers
            .Where(x =>
                x.ConversationId == conversationId &&
                x.LeftAt == null &&
                !x.IsBanned)
            .Select(x => x.UserId)
            .ToListAsync();
        }

        public async Task<List<ConversationMember>> GetActiveMembersAsync(Guid conversationId)
        {
            return await context.ConversationMembers
            .Include(x => x.User)
            .Where(x =>
                x.ConversationId == conversationId &&
                x.LeftAt == null &&
                !x.IsBanned)
            .OrderBy(x => x.JoinedAt)
            .ToListAsync();
        }

        public async Task<ConversationMember?> GetAsync(Guid conversationId, Guid userId)
        {
            return await context.ConversationMembers
             .Include(x => x.User)
             .FirstOrDefaultAsync(x =>
                 x.ConversationId == conversationId && x.UserId == userId);
        }

        public async Task<ConversationMember?> GetByIdAsync(Guid id)
        {
            return await context.ConversationMembers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Guid>> GetOtherMemberIdsAsync(Guid conversationId, Guid currentUserId)
        {
            return await context.ConversationMembers
            .Where(x =>
                x.ConversationId == conversationId &&
                x.UserId != currentUserId &&
                x.LeftAt == null &&
                !x.IsBanned)
            .Select(x => x.UserId)
            .ToListAsync();
        }

        public async Task<bool> IsActiveMemberAsync(Guid conversationId, Guid userId)
        {
            return await context.ConversationMembers
            .AnyAsync(x =>
                x.ConversationId == conversationId &&
                x.UserId == userId &&
                x.LeftAt == null &&
                !x.IsBanned);
        }

        public void Remove(ConversationMember member)
        {
            context.ConversationMembers.Remove(member);
            
        }

        public void Update(ConversationMember entity)
        {
            context.ConversationMembers.Update(entity);
        }

    }
}
