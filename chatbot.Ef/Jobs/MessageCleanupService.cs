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
    public sealed class MessageCleanupService(IUnitOfWork unitOfWork, ILogger<MessageCleanupService> logger) : IMessageCleanupService
    {
        private const int RetentionDays = 30;
        public async Task CleanupDeletedMessagesAsync(
            DateTime cutoffDate)
        {
            var deletedCount = await unitOfWork.Messages
                .DeleteExpiredMessagesAsync(cutoffDate);

            logger.LogInformation("Permanently deleted {Count} expired messages", deletedCount);
        }
    }
}
