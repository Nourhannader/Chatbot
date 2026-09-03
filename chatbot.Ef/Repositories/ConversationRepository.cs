using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class ConversationRepository(ApplicationDbContext context) : IConversationRepository
    {
        public async Task AddAsync(Conversation entity)
        {
           await context.Conversations.AddAsync(entity);
        }

        public async Task<bool> ConversationExistsAsync(Guid firstUserId, Guid secondUserId)
        {
            return await context.Conversations
                .AnyAsync(
                c=> 
                c.Type== ConversationType.OneToOne &&
                c.Members.Any(m=> m.UserId == firstUserId) &&
                c.Members.Any(m => m.UserId == secondUserId)
                );
        }

        public Task<Conversation?> GetByIdAsync(Guid id)
        {
            return context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<List<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return context.Conversations
                 .Include(c => c.Messages)
                 .Include(c => c.Members)
                 .Where(c => c.Members.Any(m => m.UserId == userId))
                 .OrderByDescending(c => c.Messages.Max(m => (DateTime?)m.SendAt))
                 .AsNoTracking()
                 .ToListAsync();
        }
        public async Task<bool> IsMemberAsync(Guid conversationId,Guid userId,CancellationToken cancellationToken = default)
        {
            return await context.ConversationMembers
                .AnyAsync(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == userId,
                    cancellationToken);
        }

        public void Update(Conversation entity)
        {
           context.Conversations.Update(entity);
        }
    }
}
