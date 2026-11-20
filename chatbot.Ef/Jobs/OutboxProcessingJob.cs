using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Outbox;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    [Queue("critical")]
    [AutomaticRetry(Attempts = 5)]
    public sealed class OutboxProcessingJob(IOutboxProcessor outboxProcessor,ILogger<OutboxProcessingJob> logger) : IOutboxProcessingJob
    {
        public async Task ProcessBatchAsync()
        {
            logger.LogInformation("Starting Outbox processing batch");

            await outboxProcessor.ProcessAsync();

            logger.LogInformation("Outbox processing batch completed");
        }
    }
}
