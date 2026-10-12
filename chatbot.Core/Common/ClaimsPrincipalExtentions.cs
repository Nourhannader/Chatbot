using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Exceptions;

namespace chatbot.Core.Common
{
    public static class ClaimsPrincipalExtentions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if(user?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedException("user is not authenticated.");
            }
            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdValue))
            {
                throw new UnauthorizedException("User id claim was not found.");
            }
            if (!Guid.TryParse(userIdValue,out var userId))
            {
                throw new UnauthorizedException("Invalid user id claim.");
            }
            return userId;
        }

    }
}
