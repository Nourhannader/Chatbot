using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Enums
{
    public enum SystemMessageType
    {
        GroupCreated,
        MemberJoined,
        MemberLeft,
        MemberRemoved,
        Promoted,
        Demoted,
        TitleChanged,
        ImageChanged,
        InviteCreated,
        InviteRevoked,
        OwnershipTransferred
    }
}
