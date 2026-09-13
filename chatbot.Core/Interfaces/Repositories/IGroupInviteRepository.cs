using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IGroupInviteRepository:IBaseRepository<GroupInvite,Guid>
    {
        Task<GroupInvite?> GetByCodeAsync(string code);
        Task<List<GroupInvite>> GetActiveInvitesAsync(Guid conversationId);
    }
}
