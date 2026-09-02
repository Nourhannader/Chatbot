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
            return await context.MessageReactions.FirstOrDefaultAsync(mr => mr.Id == id);
        }

        public async Task<List<MessageReaction>> GetMessageReactionsAsync(Guid messageId)
        {
            return await context.MessageReactions
                .Where(mr => mr.MessageId == messageId)
                .Include(mr => mr.User)
                .ToListAsync();
        }

        public async Task<MessageReaction?> GetReactionByMessageIdAndUserIdAsync(Guid messageId, Guid userId)
        {
            return await context.MessageReactions
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == userId);

        }

        public Task RemoveMessageReaction(MessageReaction reaction)
        {
             context.MessageReactions.Remove(reaction);

              return Task.CompletedTask;

        }

        public void Update(MessageReaction entity)
        {
            context.MessageReactions.Update(entity);
        }
    }
}
