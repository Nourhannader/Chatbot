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
    public sealed class ConversationRepository(ApplicationDbContext context) : IConversationRepository
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
                c.Type== ConversationType.Private &&
                !c.IsDeleted &&
                c.Members.Count(m => m.LeftAt == null) == 2 &&
                c.Members.Any(m=> m.UserId == firstUserId && m.LeftAt == null) &&
                c.Members.Any(m => m.UserId == secondUserId && m.LeftAt == null)
                );
        }

        public void Delete(Conversation conversation)
        {
            context.Conversations.Remove(conversation);
        }

        public Task<Conversation?> GetByIdAsync(Guid id)
        {
            return context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public Task<Conversation?> GetDetailsAsync(Guid conversationId)
        {
            return context.Conversations
            .Include(x => x.Image)
            .Include(x => x.Members)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x =>  x.Id == conversationId && !x.IsDeleted);
        }

        public Task<Conversation?> GetGroupWithMembersAsync(Guid ConversationId)
        {
            return context.Conversations
                .Include(c => c.Members)
                .FirstOrDefaultAsync(x =>
                x.Id== ConversationId && x.Type == ConversationType.Group
                && !x.IsDeleted
                );
        }

        public Task<Conversation?> GetPrivateAsync(Guid firstUserId, Guid secondUserId)
        {
            return context.Conversations
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.Type == ConversationType.Private &&
                x.Members.Count(m =>
                    m.LeftAt == null &&
                    !m.IsBanned) == 2 &&
                x.Members.Any(m =>
                    m.UserId == firstUserId &&
                    m.LeftAt == null &&
                    !m.IsBanned) &&
                x.Members.Any(m =>
                    m.UserId == secondUserId &&
                    m.LeftAt == null &&
                    !m.IsBanned));
        }

        public Task<List<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return context.Conversations
           .Where(c => !c.IsDeleted &&
               c.Members.Any(m =>
                   m.UserId == userId &&
                   m.LeftAt == null &&
                   !m.IsBanned))
           .Include(c => c.Image)
           .Include(c => c.Members)
               .ThenInclude(m => m.User)
           .AsNoTracking()
           .ToListAsync();
        }

        public Task<Conversation?> GetWithImageAsync(Guid conversationId)
        {
            return context.Conversations
           .Include(x => x.Image)
           .FirstOrDefaultAsync(x => x.Id == conversationId && !x.IsDeleted);
        }

        public void Update(Conversation entity)
        {
           context.Conversations.Update(entity);
        }
    
        public Task<bool> ExistsAsync(Guid conversationId)
        {
            return context.Conversations
                .AnyAsync(x =>
                    x.Id == conversationId &&
                    !x.IsDeleted);
        }

        public async Task<Conversation?> GetPrivateByKeyAsync(string privateKey)
        {
            return await context.Conversations
                .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Type == ConversationType.Private &&
                    x.PrivateConversationKey == privateKey);
        }
    }
}
