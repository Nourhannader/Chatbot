using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services.CleanUp
{
    public interface IOutboxCleanupService
    {
        Task DeleteProcessedOlderThanAsync(DateTime cutoffDate);
    }
}
