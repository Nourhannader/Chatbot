using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Authorization.Conversation
{
    public static class ConversationRolePermissions
    {
        public static readonly IReadOnlyDictionary<ConversationRole,HashSet<string>> Permissions =
            new Dictionary<ConversationRole, HashSet<string>>
            {
                [ConversationRole.Member] = new()
                {
                ConversationPermissions.View,
                ConversationPermissions.SendMessage,
                ConversationPermissions.Reply,
                ConversationPermissions.React,
                ConversationPermissions.UploadFile
                },

                [ConversationRole.Admin] = new()
                {
                ConversationPermissions.View,
                ConversationPermissions.SendMessage,
                ConversationPermissions.Reply,
                ConversationPermissions.React,
                ConversationPermissions.UploadFile,

                ConversationPermissions.ManageMessages,
                ConversationPermissions.DeleteMessage,

                ConversationPermissions.AddMember,
                ConversationPermissions.RemoveMember,

                ConversationPermissions.ManageGroup,

                ConversationPermissions.MuteMember,
                ConversationPermissions.UnmuteMember,

                ConversationPermissions.BanMember,
                ConversationPermissions.UnbanMember
                },

                [ConversationRole.Owner] = new()
                {
                ConversationPermissions.View,
                ConversationPermissions.SendMessage,
                ConversationPermissions.Reply,
                ConversationPermissions.React,
                ConversationPermissions.UploadFile,

                ConversationPermissions.ManageMessages,
                ConversationPermissions.DeleteMessage,

                ConversationPermissions.AddMember,
                ConversationPermissions.RemoveMember,

                ConversationPermissions.ManageGroup,

                ConversationPermissions.MuteMember,
                ConversationPermissions.UnmuteMember,

                ConversationPermissions.BanMember,
                ConversationPermissions.UnbanMember,

                ConversationPermissions.PromoteMember,
                ConversationPermissions.DemoteMember,

                ConversationPermissions.TransferOwnership,
                ConversationPermissions.DeleteGroup
                }
            };
    }
}
