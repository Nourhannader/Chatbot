using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface ICurrentUser
    {
        Guid UserId { get; }

        ClaimsPrincipal Principal { get; }

        bool IsAuthenticated { get; }
    }
}
