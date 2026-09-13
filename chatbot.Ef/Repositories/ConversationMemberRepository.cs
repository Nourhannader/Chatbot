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
    public class ConversationMemberRepository(ApplicationDbContext context) : IConversationMemberRepository
    {
        public async Task AddAsync(ConversationMember entity)
        {
            await context.ConversationMembers.AddAsync(entity);
        }

        public async Task<bool> ExistsAsync(Guid conversationId, Guid userId)
        {
          return await context.ConversationMembers
                .AnyAsync(x => x.ConversationId == conversationId && x.UserId == userId && x.LeftAt ==null);   
        }

        public async Task<List<ConversationMember>> GetActiveMembersAsync(Guid conversationId)
        {
            return await context.ConversationMembers
                .Where(x => x.ConversationId == conversationId && x.LeftAt == null)
                .ToListAsync();
        }

        public async Task<ConversationMember?> GetAsync(Guid conversationId, Guid userId)
        {
            return await context.ConversationMembers
                .FirstOrDefaultAsync(x => x.ConversationId == conversationId && x.UserId == userId);
        }

        public async Task<ConversationMember?> GetByIdAsync(Guid id)
        {
            return await context.ConversationMembers.FirstOrDefaultAsync(x => x.Id == id);
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
