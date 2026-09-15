using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Data;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Jobs
{
    public class NotificationCleanupJob(IUnitOfWork unitOfWork)
    {
        public async Task ExecuteAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-90);
            var notifications =await unitOfWork.Notifications.GetNotificationsReaded(cutoff);
            if (notifications.Count == 0)
            {
                return;
            }
            await unitOfWork.Notifications.RemoveRange(notifications);
            await unitOfWork.SaveChangesAsync();
        }
       
    }
}
