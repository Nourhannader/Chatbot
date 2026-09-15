using chatbot.Ef.Jobs;
using Hangfire;

namespace chatbot.Api.Extensions
{
    public static class JobScheduler
    {
        public static void Register()
        {
            RecurringJob.AddOrUpdate<FileCleanupJob>(
                "file-cleanup",
                job => job.ExecuteAsync(),
                Cron.Daily);

            RecurringJob.AddOrUpdate<NotificationCleanupJob>(
                "notification-cleanup",
                job => job.ExecuteAsync(),
                Cron.Weekly);

            RecurringJob.AddOrUpdate<SessionCleanupJob>(
                "session-cleanup",
                job => job.ExecuteAsync(),
                Cron.Daily);

            RecurringJob.AddOrUpdate<MessageCleanupJob>(
                "message-cleanup",
                job => job.ExecuteAsync(),
                Cron.Daily);
        }
    }
}
