using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services.CleanUp;
using chatbot.Core.Interfaces.UnitOFWork;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    [Queue("maintenance")]
    [AutomaticRetry(Attempts = 3)]
    [DisableConcurrentExecution(timeoutInSeconds: 600)]
    public sealed class MessageCleanupJob(IMessageCleanupService cleanupService) : IMessageCleanupJob
    {
        public Task CleanupAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            return cleanupService.CleanupDeletedMessagesAsync(cutoffDate);
        }
    }
}
