using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services;
using Hangfire;

namespace chatbot.Ef.Jobs
{
    [Queue("maintenance")]
    [AutomaticRetry(Attempts = 3)]
    public sealed class GroupInviteExpirationJob(IGroupInviteService inviteService) : IGroupInviteExpirationJob
    {
        public async Task ExpireAsync(Guid inviteId)
        {
            await inviteService.ExpireIfNeededAsync(inviteId);
        }
    }
}
