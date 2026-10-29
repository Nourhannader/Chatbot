using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        

        public ClaimsPrincipal Principal => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

        public bool IsAuthenticated => Principal.Identity?.IsAuthenticated == true;

        public Guid UserId
        {
            get
            {
                var userId = Principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var id))
                {
                    throw new UnauthorizedAccessException("Authenticated user id was not found.");
                }

                return id;
            }
        }
    }
}
