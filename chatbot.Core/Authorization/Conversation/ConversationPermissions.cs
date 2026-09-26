using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Authorization.Conversation
{
    public static class ConversationPermissions
    {
        public const string SendMessage = "conversation.message.send";

        public const string Reply = "conversation.message.reply";

        public const string React = "conversation.message.react";

        public const string UploadFile = "conversation.file.upload";

        public const string ManageMessages = "conversation.message.manage";

        public const string DeleteMessage = "conversation.message.delete";

        public const string AddMember = "conversation.member.add";

        public const string RemoveMember = "conversation.member.remove";

        public const string PromoteMember = "conversation.member.promote";

        public const string DemoteMember = "conversation.member.demote";

        public const string ManageGroup = "conversation.group.manage";

        public const string DeleteGroup = "conversation.group.delete";

        public const string TransferOwnership = "conversation.group.transferOwnership";

        public const string MuteMember = "conversation.member.mute";

        public const string UnmuteMember = "conversation.member.unmute";

        public const string BanMember = "conversation.member.ban";

        public const string UnbanMember = "conversation.member.unban";
    }
}
