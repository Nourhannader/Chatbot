using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services.CleanUp;
using chatbot.Core.Interfaces.UnitOFWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public sealed class NotificationCleanupService(IUnitOfWork unitOfWork, ILogger<NotificationCleanupService> logger) : INotificationCleanupService
    {
        private const int RetentionDays = 90;

        public async Task CleanupOldNotificationsAsync( DateTime cutoffDate)
        {
            var deletedCount = await unitOfWork.Notifications
                .DeleteReadOlderThanAsync(cutoffDate);

            logger.LogInformation("Deleted {Count} old read notifications", deletedCount);
        }
    }
}
