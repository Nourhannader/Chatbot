using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.BackgroundJobs
{
    public interface IBackgroundJobScheduler
    {
        string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall) where TJob : class;

        string Schedule<TJob>( Expression<Func<TJob, Task>> methodCall, TimeSpan delay) where TJob : class;

        string ScheduleAt<TJob>(Expression<Func<TJob, Task>> methodCall, DateTime enqueueAt) where TJob : class;
    }
}
