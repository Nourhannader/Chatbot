using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Jobs
{
    public class SessionCleanupJob(IUnitOfWork unitOfWork)
    {
        public async Task ExecuteAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var sessions = await unitOfWork.Sessions.GetAllRevokedSessionOlder(cutoff);
            if (sessions.Count == 0)
                return;
            unitOfWork.Sessions.RemoveRange(sessions);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
