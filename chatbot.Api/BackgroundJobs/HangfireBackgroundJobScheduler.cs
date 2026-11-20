using System.Linq.Expressions;
using chatbot.Core.Interfaces.BackgroundJobs;
using Hangfire;

namespace chatbot.Api.BackgroundJobs
{
    public class HangfireBackgroundJobScheduler(IBackgroundJobClient client) : IBackgroundJobScheduler
    {
        public string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall) where TJob : class
        {
            return client.Enqueue<TJob>(methodCall);
        }

        public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay) where TJob : class
        {
            if (delay < TimeSpan.Zero)
                delay = TimeSpan.Zero;

            return client.Schedule(methodCall, delay);
        }

        public string ScheduleAt<TJob>(Expression<Func<TJob, Task>> methodCall, DateTime enqueueAt) where TJob : class
        {
            var utcTime = enqueueAt.Kind == DateTimeKind.Utc
            ? enqueueAt : enqueueAt.ToUniversalTime();

            return client.Schedule(methodCall, utcTime);
        }
    }
}
