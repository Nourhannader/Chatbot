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
    public sealed class GroupRepository(ApplicationDbContext context) : IGroupRepository
    {
        public async Task<Conversation?> GetByIdAsync(Guid conversationId)
        {
            return await context.Conversations
            .FirstOrDefaultAsync(x =>
                x.Id == conversationId &&
                x.Type == ConversationType.Group &&
                !x.IsDeleted);
        }

        public async Task<Conversation?> GetWithImageAsync(Guid conversationId)
        {
            return await context.Conversations
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x =>
                x.Id == conversationId &&
                x.Type == ConversationType.Group &&
                !x.IsDeleted);
        }

        public async Task<Conversation?> GetWithMembersAsync(Guid conversationId)
        {
            return await context.Conversations
            .Include(x => x.Members)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Id == conversationId &&
                x.Type == ConversationType.Group &&
                !x.IsDeleted);
        }
    }
}
