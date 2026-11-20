using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services.CleanUp;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public sealed class OutboxCleanupService(IUnitOfWork unitOfWork,ILogger<OutboxCleanupService> logger) : IOutboxCleanupService
    {
        public async Task DeleteProcessedOlderThanAsync( DateTime cutoffDate)
        {
            var deletedCount = await unitOfWork.OutboxMessages
                .DeleteProcessedOlderThanAsync(cutoffDate);

            logger.LogInformation("Deleted {Count} processed outbox messages older than {Date}", deletedCount, cutoffDate);
        }
    }
}
