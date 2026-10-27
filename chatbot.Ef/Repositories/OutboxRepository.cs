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
    public sealed class OutboxRepository(ApplicationDbContext context) : IOutboxRepository
    {
        

        public async Task AddAsync( OutboxMessage message, CancellationToken cancellationToken = default)
        {
            await context.OutboxMessages.AddAsync(
                message,
                cancellationToken);
        }

        public async Task<List<OutboxMessage>> GetPendingAsync( int batchSize,DateTime utcNow,CancellationToken cancellationToken = default)
        {
            return await context.OutboxMessages
                .Where(x =>
                    (x.Status == OutboxStatus.Pending ||
                     x.Status == OutboxStatus.Failed) &&
                    (x.NextRetryAt == null ||
                     x.NextRetryAt <= utcNow))
                .OrderBy(x => x.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<OutboxMessage?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default)
        {
            return await context.OutboxMessages
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<List<OutboxMessage>> GetDeadLettersAsync(int take = 100, CancellationToken cancellationToken = default)
        {
            return await context.OutboxMessages
                .AsNoTracking()
                .Where(x =>
                    x.Status == OutboxStatus.DeadLetter)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
