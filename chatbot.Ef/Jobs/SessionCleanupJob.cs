using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.UnitOFWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class SessionCleanupJob(IUnitOfWork unitOfWork,ILogger<SessionCleanupJob> logger)
    {
        public async Task ExecuteAsync()
        {
            logger.LogInformation("Session CleanUp Started.");
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var sessions = await unitOfWork.Sessions.GetAllRevokedSessionOlder(cutoff);
            if (!sessions.Any())
            {
                logger.LogInformation("No old Session Found.");
                return;
            }
            unitOfWork.Sessions.RemoveRange(sessions);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Deleted {Count} old Sessions.", sessions.Count);
        }
    }
}
