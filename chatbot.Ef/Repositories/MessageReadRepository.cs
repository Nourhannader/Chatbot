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
    public class MessageReadRepository(ApplicationDbContext context) : IMessageReadRepository
    {
        public async Task AddAsync(MessageRead entity)
        {
             await context.MessageReads.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<MessageRead> messageReads)
        {
            await context.MessageReads.AddRangeAsync(messageReads);
        }

        public async Task<bool> ExistsAsync(Guid messageId, Guid userId)
        {
            return await context.MessageReads.AnyAsync(mr => mr.MessageId == messageId && mr.UserId == userId);
        }

        public async Task<MessageRead?> GetByIdAsync(Guid id)
        {
            return await context.MessageReads
                .AsNoTracking()
                .FirstOrDefaultAsync(mr => mr.Id==id);
        }

        public async Task<MessageRead?> GetByMessageAndUserAsync(Guid messageId, Guid userId)
        {
            return await context.MessageReads
                .AsNoTracking()
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == userId);
        }

        public async Task<IEnumerable<MessageRead>> GetByMessageIdAsync(Guid messageId)
        {
            return await context.MessageReads
                .AsNoTracking()
                .Where(mr => mr.MessageId == messageId)
                .OrderBy(mr => mr.ReadAt)
                .ToListAsync();
        }

        public void Update(MessageRead entity)
        {
            context.MessageReads.Update(entity);
        }
    }
}
