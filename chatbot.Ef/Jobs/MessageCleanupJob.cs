using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.UnitOFWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class MessageCleanupJob(IUnitOfWork unitOfWork,ILogger<MessageCleanupJob> logger)
    {
        public async Task ExecuteAsync()
        {
            logger.LogInformation("Message CleanUp job Started.");
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var messages = await unitOfWork.Messages.GetAllMessageDeletedOlder(cutoff);
            if (!messages.Any())
            {
                logger.LogInformation("No old deleted messages found");
                return;
            }

            unitOfWork.Messages.RemoveRange(messages);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Deleted {Count} old Messages.", messages.Count);
        }
    }
}
