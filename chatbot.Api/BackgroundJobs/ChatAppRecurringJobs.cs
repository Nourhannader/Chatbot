using chatbot.Core.Interfaces.BackgroundJobs;
using Hangfire;

namespace chatbot.Api.BackgroundJobs
{
    public static class ChatAppRecurringJobs
    {
        private static readonly TimeZoneInfo Utc = TimeZoneInfo.Utc;

        public static void Register(IRecurringJobManager recurringJobs)
        {
            // 1. Process Outbox - Every Minute
            recurringJobs.AddOrUpdate<IOutboxProcessingJob>(
                "outbox-processing",
                job => job.ProcessBatchAsync(),
                Cron.Minutely,
                new RecurringJobOptions
                {
                    TimeZone = Utc
                });

            // 2. Expire Group Invites - Every Hour
            recurringJobs.AddOrUpdate<IChatMaintenanceJob>(
                "expire-group-invites",
                job => job.ExpireInvitesAsync(),
                Cron.Hourly,
                new RecurringJobOptions
                {
                    TimeZone = Utc
                });

            // 3. Cleanup Processed Outbox - Daily at 02:00 UTC
            recurringJobs.AddOrUpdate<IChatMaintenanceJob>(
                "cleanup-processed-outbox",
                job => job.CleanupProcessedOutboxAsync(),
                "0 2 * * *",
                new RecurringJobOptions
                {
                    TimeZone = Utc
                });

            // 4. Cleanup Orphaned Files - Daily at 03:00 UTC
            recurringJobs.AddOrUpdate<IChatMaintenanceJob>(
                "cleanup-orphaned-files",
                job => job.CleanupOrphanedFilesAsync(),
                "0 3 * * *",
                new RecurringJobOptions
                {
                    TimeZone = Utc
                });

            // 5. Session Cleanup - Daily at 03:30 UTC
            recurringJobs.AddOrUpdate<ISessionCleanupJob>(
                "session-cleanup",
                job => job.CleanupAsync(),
                "30 3 * * *",
                new RecurringJobOptions
                {
                    TimeZone = Utc
                });

            // 6. Notification Cleanup - Daily at 04:00 UTC
            recurringJobs.AddOrUpdate<INotificationCleanupJob>(
                "notification-cleanup",
                job => job.CleanupAsync(),
                "0 4 * * *",
                new RecurringJobOptions
                {
                    TimeZone = Utc
                });

            // 7. Message Cleanup - Daily at 04:30 UTC
            recurringJobs.AddOrUpdate<IMessageCleanupJob>(
                "message-cleanup",
                job => job.CleanupAsync(),
                "30 4 * * *",
                new RecurringJobOptions
                {
                    TimeZone = Utc
                });
        }
    }
}
