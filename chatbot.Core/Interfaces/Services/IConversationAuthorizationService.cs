using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IConversationAuthorizationService
    {
        Task AuthorizeAsync(ClaimsPrincipal user, Guid conversationId, string permission);

        //Task<bool> HasPermissionAsync(Guid conversationId, Guid userId, string permission);

        //Task<bool> IsOwnerAsync(Guid conversationId, Guid userId);

        //Task<bool> IsAdminAsync(Guid conversationId, Guid userId);

        //Task<bool> IsMemberAsync(Guid conversationId, Guid userId);
    }
}
