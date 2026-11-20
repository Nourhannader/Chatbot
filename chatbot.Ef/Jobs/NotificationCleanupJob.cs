using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services.CleanUp;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Data;
using chatbot.Ef.UnitOfWork;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    [Queue("maintenance")]
    [AutomaticRetry(Attempts = 3)]
    public sealed class NotificationCleanupJob(INotificationCleanupService cleanupService) : INotificationCleanupJob
    {

        public Task CleanupAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-90);

            return cleanupService.CleanupOldNotificationsAsync(cutoffDate);
        }
    }
}
