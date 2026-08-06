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
    public class MessageRepository(ApplicationDbContext context) : IMessageRepository
    {
        public async Task AddAsync(Message entity)
        {
            await context.Messages.AddAsync(entity);
        }

        public async Task<Message> GetByIdAsync(string id)
        {
            return await context.Messages
                .Include(m=>m.Reactions)
                .Include(m=> m.RecipientStatuses)
                .FirstOrDefaultAsync(m => m.Id == id)
                ;
        }

        public async Task<List<Message>> GetConversationMessagesAsync(string conversationId, int page, int pageSize)
        {
           return await context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Reactions)
                .Include(m => m.RecipientStatuses)
                .ToListAsync();
        }

        public void Update(Message entity)
        {
            context.Messages.Update(entity);
        }
    }
}
