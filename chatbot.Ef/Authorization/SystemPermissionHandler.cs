using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.System;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Ef.Authorization
{
    public class SystemPermissionHandler(UserManager<ApplicationUser> userManager) : AuthorizationHandler<SystemPermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemPermissionRequirement requirement)
        {
            {
                if (context.User.Identity?.IsAuthenticated != true)
                    return;

                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userId, out var id))
                    return;

                var user = await userManager.FindByIdAsync(id.ToString());

                if (user == null)
                    return;

                var roles = await userManager.GetRolesAsync(user);

                foreach (var role in roles)
                {
                    if (SystemRolePermissions.Permissions
                        .TryGetValue(role, out var permissions)
                        &&
                        permissions.Contains(
                            requirement.Permission))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
        }
    }
}
