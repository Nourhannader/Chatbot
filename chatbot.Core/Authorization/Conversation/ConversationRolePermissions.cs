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
        private static readonly HashSet<string>
        MemberPermissions =
    [
        ConversationPermissions.View,
        ConversationPermissions.SendMessage,
        ConversationPermissions.Reply,
        ConversationPermissions.React,
        ConversationPermissions.UploadFile,
        ConversationPermissions.DeleteOwnMessage
    ];


        private static readonly HashSet<string>
            AdminPermissions =
        [
            .. MemberPermissions,

        ConversationPermissions.ManageMessages,

        ConversationPermissions.AddMember,
        ConversationPermissions.RemoveMember,

        ConversationPermissions.MuteMember,
        ConversationPermissions.UnmuteMember,

        ConversationPermissions.BanMember,
        ConversationPermissions.UnbanMember,

        ConversationPermissions.ManageGroup,
        ConversationPermissions.ManageInvites
        ];


        private static readonly HashSet<string>
            OwnerPermissions =
        [
            .. AdminPermissions,

        ConversationPermissions.PromoteMember,
        ConversationPermissions.DemoteMember,

        ConversationPermissions.TransferOwnership,

        ConversationPermissions.DeleteGroup
        ];


        public static IReadOnlySet<string>
            GetPermissions(
                ConversationRole role)
        {
            return role switch
            {
                ConversationRole.Member =>
                    MemberPermissions,

                ConversationRole.Admin =>
                    AdminPermissions,

                ConversationRole.Owner =>
                    OwnerPermissions,

                _ => new HashSet<string>()
            };
        }
    }
}
