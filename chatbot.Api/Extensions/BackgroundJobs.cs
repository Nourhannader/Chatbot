using chatbot.Api.BackgroundJobs;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Ef.Jobs;
using Hangfire;
using ServiceStack;

namespace chatbot.Api.Extensions
{
    public static class BackgroundJobs
    {
        public static IServiceCollection AddHangfireConfig(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddHangfire(configuration =>
            {
                configuration
                    .SetDataCompatibilityLevel(
                        CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
            });

            Services.AddHangfireServer(options =>
            {
                options.WorkerCount = 5;

                options.Queues =
                [
                  "critical",
                  "notifications",
                  "emails",
                  "media",
                  "maintenance",
                  "default"
                ];
            });


            //scheduler
            Services.AddScoped<IBackgroundJobScheduler, HangfireBackgroundJobScheduler>();
            //background jobs

            Services.AddScoped<IPushNotificationJob,PushNotificationJob>();

            Services.AddScoped< IMediaProcessingJob, MediaProcessingJob>();

            Services.AddScoped<IOutboxProcessingJob,OutboxProcessingJob>();

            Services.AddScoped< IGroupInviteExpirationJob,GroupInviteExpirationJob>();

            Services.AddScoped< IChatMaintenanceJob, ChatMaintenanceJob>();

            Services.AddScoped<ISessionCleanupJob, SessionCleanupJob>();

            Services.AddScoped<INotificationCleanupJob, NotificationCleanupJob>();

            Services.AddScoped<IMessageCleanupJob, MessageCleanupJob>();

            Services.AddScoped<IMediaJobDispatcher, MediaJobDispatcher>();

            return Services;
        }
    }
}
