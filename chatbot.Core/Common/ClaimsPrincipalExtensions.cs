using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Exceptions;

namespace chatbot.Core.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedException("user is not authenticated.");
            }
            var value = user.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value) ||
                !Guid.TryParse(value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user.");
            }

            return userId;
        }
    }
}
