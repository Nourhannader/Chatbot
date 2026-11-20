using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.BackgroundJobs
{
    public interface IGroupInviteExpirationJob
    {
        Task ExpireAsync(Guid inviteId);
    }
}
