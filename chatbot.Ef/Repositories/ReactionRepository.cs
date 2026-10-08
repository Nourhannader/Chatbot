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
    public class ReactionRepository(ApplicationDbContext context) : IReactionRepository
    {
        public async Task AddAsync(MessageReaction entity)
        {
            await context.MessageReactions.AddAsync(entity);
        }

        public async Task<MessageReaction?> GetByIdAsync(Guid id)
        {
            return await context.MessageReactions.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<MessageReaction?> GetByMessageAndUserAsync(Guid messageId, Guid userId)
        {
            return await context.MessageReactions.FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);
        }

        public async Task<List<MessageReaction>> GetByMessageIdAsync(Guid messageId)
        {
            return await context.MessageReactions.Where(r => r.MessageId == messageId).ToListAsync();
        }

        public void Remove(MessageReaction reaction)
        {
            context.MessageReactions.Remove(reaction);
        }

        public void Update(MessageReaction entity)
        {
            context.MessageReactions.Update(entity);
        }
    }
}
