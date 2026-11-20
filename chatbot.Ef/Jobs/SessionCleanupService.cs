using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services.CleanUp;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class SessionCleanupService(IUnitOfWork unitOfWork,ILogger<SessionCleanupService> logger) : ISessionCleanupService
    {
        private const int RetentionDays = 30;
        public async Task CleanupExpiredSessionsAsync(DateTime cutoffDate)
        {
            var deletedCount = await unitOfWork.Sessions
            .DeleteExpiredSessionsAsync(cutoffDate);

            logger.LogInformation( "Deleted {Count} expired or revoked sessions",deletedCount);
        }
    }
}
