using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IOutboxRepository
    {
        Task AddAsync( OutboxMessage message, CancellationToken cancellationToken = default);

        Task<List<OutboxMessage>> GetPendingAsync(int batchSize,DateTime utcNow, CancellationToken cancellationToken = default);

        Task<OutboxMessage?> GetByIdAsync( Guid id, CancellationToken cancellationToken = default);

        Task<List<OutboxMessage>> GetDeadLettersAsync( int take = 100,CancellationToken cancellationToken = default);
    }
}
