using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Data;
using chatbot.Ef.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class NotificationCleanupJob(IUnitOfWork unitOfWork,ILogger<NotificationCleanupJob> logger)
    {
        public async Task ExecuteAsync()
        {
            logger.LogInformation("Notification CleanUp Started.");
            var cutoff = DateTime.UtcNow.AddDays(-90);
            var notifications =await unitOfWork.Notifications.GetNotificationsReaded(cutoff);
            if (!notifications.Any())
            {
                logger.LogInformation("No old read Notifications found.");
                return;
            }
            unitOfWork.Notifications.RemoveRange(notifications);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Deleted {Count} old read Notifications.",notifications.Count);
        }
       
    }
}
