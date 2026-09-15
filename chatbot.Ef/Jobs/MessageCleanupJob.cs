using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Jobs
{
    public class MessageCleanupJob(IUnitOfWork unitOfWork)
    {
        public async Task ExecuteAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var messages = await unitOfWork.Messages.GetAllMessageDeletedOlder(cutoff);
            if (messages.Count == 0)
                return;

            unitOfWork.Messages.RemoveRange(messages);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
