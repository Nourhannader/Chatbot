using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Enums
{
    public enum NotificationType
    {
        MemberAdded = 1,
        MemberRemoved = 2,
        MemberBanned = 3,
        MemberUnbanned = 4,
        MemberPromoted = 5,
        MemberDemoted = 6,
        OwnershipTransferred = 7,
        MemberMuted = 8,
        MemberUnmuted = 9,
        NewMessage = 10,
        MessageReaction = 11,
        MessageReply = 12,
        Mention = 13


    }
}
