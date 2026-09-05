using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Enums
{
    public enum NotificationType
    {
        NewMessage = 1,
        MessageReaction = 2,
        MessageReply = 3,
        AddedToGroup = 4,
        RemovedFromGroup = 5,
        Mention = 6,
        System=7,
        FriendRequest = 8


    }
}
