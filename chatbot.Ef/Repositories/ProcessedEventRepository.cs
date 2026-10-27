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
    public sealed class ProcessedEventRepository(ApplicationDbContext context) : IProcessedEventRepository
    {
        

        public Task<bool> ExistsAsync(Guid eventId,string handlerName,CancellationToken cancellationToken = default)
        {
            return context.ProcessedEvents
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.EventId == eventId &&
                        x.HandlerName == handlerName,
                    cancellationToken);
        }

        public async Task AddAsync(ProcessedEvent processedEvent,CancellationToken cancellationToken = default)
        {
            await context.ProcessedEvents.AddAsync( processedEvent,cancellationToken);
        }
    }
}
