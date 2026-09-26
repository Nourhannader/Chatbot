using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;

namespace chatbot.Core.Interfaces.Services
{
    public interface IConversationAuthorizationService
    {
        Task AuthorizeAsync(ClaimsPrincipal user, Guid conversationId, string permission);

        Task AuthorizeAsync(ClaimsPrincipal user, ConversationResource resource, string permission);

        Task AuthorizeMessageAsync(ClaimsPrincipal user,Guid conversationId,Guid messageId,string permission);
    }
}
