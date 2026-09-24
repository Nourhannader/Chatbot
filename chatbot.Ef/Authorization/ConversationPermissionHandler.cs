using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Interfaces.Services;
using chatbot.Ef.Services;
using Microsoft.AspNetCore.Authorization;

namespace chatbot.Ef.Authorization
{
    public class ConversationPermissionHandler(IRolePermissionService rolePermission) : AuthorizationHandler<PermissionRequirement, ConversationResource>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement, ConversationResource resource)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return;

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return;
            }

            var allowed = await rolePermission.HasPermissionAsync(
                        userId, resource.ConversationId, requirement.Permission);

            if (allowed)
            {
                context.Succeed(requirement);
            }
        }
    }
}
