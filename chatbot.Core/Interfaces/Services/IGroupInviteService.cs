using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IGroupInviteService
    {
        Task<string> GenerateAsync(Guid conversationId, Guid userId);

        Task JoinAsync(string code, Guid userId);

        Task RevokeAsync(Guid inviteId, Guid userId);
    }
}
