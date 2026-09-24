using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Asn1.Cmp;

namespace chatbot.Ef.Authorization
{
    public class ConversationAuthorizationService(IAuthorizationService authorizationService) : IConversationAuthorizationService
    {
        public async Task AuthorizeAsync(ClaimsPrincipal user, Guid conversationId, string permission)
        {
            var requirement = new PermissionRequirement(permission);
            var resource = new ConversationResource(conversationId);

            var result = await authorizationService.AuthorizeAsync(
                user, resource, requirement);

            if (!result.Succeeded)
            {
                throw new ForbiddenException("You are not authorized to perform this action.");
            }
        }
    }
}
