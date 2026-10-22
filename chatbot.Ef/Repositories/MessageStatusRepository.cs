using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class MessageStatusRepository(ApplicationDbContext context) : IMessageStatusRepository
    {
        public async Task AddRangeAsync(IEnumerable<MessageRecipientStatus> recipients)
        {
            await context.MessageRecipientStatuses.AddRangeAsync(recipients);
        }

        public async Task<bool> ExistsAsync(Guid messageId, Guid userId)
        {
            return await context.MessageRecipientStatuses
                .AnyAsync(x => x.MessageId == messageId && x.RecipientId == userId);
        }

        public async Task<MessageRecipientStatus?> GetAsync(Guid messageId, Guid userId)
        {
            return await context.MessageRecipientStatuses
                .Include(x => x.Message)
                .Include(x => x.Recipient)
                .FirstOrDefaultAsync(x => x.MessageId == messageId && x.RecipientId == userId);
        }

        public async Task<List<MessageRecipientStatus>> GetByMessageIdAsync(Guid messageId)
        {
            return await context.MessageRecipientStatuses
                .AsNoTracking()
                .Where(x => x.MessageId == messageId)
                .ToListAsync();
        }

        public async Task<MessageStatusSummaryDto?> GetSummaryAsync(Guid messageId)
        {
            return await context.MessageRecipientStatuses
            .AsNoTracking()
            .Where(x => x.MessageId == messageId)
            .GroupBy(x => x.MessageId)
            .Select(g => new MessageStatusSummaryDto
            {
                MessageId = g.Key,
                RecipientCount = g.Count(),
                SentCount = g.Count(x => x.Status == MessageStatus.Sent),
                DeliveredCount = g.Count(x => x.DeliveredAt != null),
                ReadCount = g.Count(x => x.ReadAt != null)
            })
            .FirstOrDefaultAsync();
        }

        public async Task<List<MessageRecipientStatus>> GetUnreadAsync(Guid userId, Guid conversationId, DateTime until)
        {
            return await context.MessageRecipientStatuses
            .Include(x => x.Message)
            .Where(x =>
                x.RecipientId == userId &&
                x.Message.ConversationId == conversationId &&
                x.ReadAt == null &&
                x.Message.SendAt <= until)
            .ToListAsync();
        }
    }
}
