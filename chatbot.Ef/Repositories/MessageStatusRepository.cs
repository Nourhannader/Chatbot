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
    public class MessageStatusRepository(ApplicationDbContext context) : IMessageStatusRepository
    {
        public async Task AddAsync(MessageRecipientStatus entity)
        {
            await context.MessageRecipientStatuses.AddAsync(entity);
        }

        public async Task<MessageRecipientStatus?> GetAsync(string messageId, string recipientId)
        {
            return await context.MessageRecipientStatuses
                .FirstOrDefaultAsync(mrs => mrs.MessageId == messageId && mrs.RecipientId == recipientId);
        }

        public async Task<MessageRecipientStatus?> GetByIdAsync(string id)
        {
            return await context.MessageRecipientStatuses
                .FirstOrDefaultAsync(mrs => mrs.MessageId == id);
        }

        public async Task<List<MessageRecipientStatus>> GetByMessageAsync(string messageId)
        {
            return await context.MessageRecipientStatuses
                .Where(mrs => mrs.MessageId==messageId)
                .ToListAsync();
        }

        public void Update(MessageRecipientStatus entity)
        {
            context.MessageRecipientStatuses.Update(entity);
        }
    }
}
