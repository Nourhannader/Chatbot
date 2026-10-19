using System.Security.Claims;

namespace chatbot.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new InvalidOperationException("User ID claim not found.");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                throw new InvalidOperationException("User ID claim is not a valid GUID.");

            return userId;
        }
    }
}
using chatbot.Core.Extensions; // Add this using directive at the top of the file
