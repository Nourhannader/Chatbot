using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services.CleanUp;
using chatbot.Core.Interfaces.UnitOFWork;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace chatbot.Ef.Jobs
{
    public class SessionCleanupJob(ISessionCleanupService sessionCleanup) : ISessionCleanupJob
    {
        public Task CleanupAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            return sessionCleanup.CleanupExpiredSessionsAsync( cutoffDate);
        }
    }
}
