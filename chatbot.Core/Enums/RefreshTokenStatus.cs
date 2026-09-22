using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Enums
{
    public enum RefreshTokenStatus
    {
        Valid,
        NotFound,
        Expired,
        Revoked,
        ReuseDetected,
        SessionRevoked

    }
}
