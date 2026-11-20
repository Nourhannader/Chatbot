using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services;
using Hangfire;

namespace chatbot.Ef.Jobs
{
    [Queue("maintenance")]
    [AutomaticRetry(Attempts = 3)]
    public sealed class ChatMaintenanceJob(IOutboxCleanupService outboxCleanup, IGroupInviteService inviteService, IOrphanFileCleanupService fileCleanup) : IChatMaintenanceJob
    {

        public Task CleanupProcessedOutboxAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-7);

            return outboxCleanup.DeleteProcessedOlderThanAsync(cutoff);
        }

        public Task ExpireInvitesAsync()
        {
            return inviteService.ExpireAllDueInvitesAsync();
        }

        public Task CleanupOrphanedFilesAsync()
        {
            return fileCleanup.CleanupAsync();
        }
    }
}
