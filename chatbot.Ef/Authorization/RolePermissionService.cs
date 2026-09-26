using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Authorization
{
    public class RolePermissionService(IUnitOfWork unitOfWork) : IRolePermissionService
    {
        private static bool HasPermission(ConversationRole role,string permission)
        {
            return role switch
            {
                ConversationRole.Member => IsMemberPermission(permission),

                ConversationRole.Admin => IsAdminPermission(permission),

                ConversationRole.Owner => IsOwnerPermission(permission),

                _ => false
            };
        }

        private static bool IsMemberPermission(string permission)
        {
            return permission == ConversationPermissions.SendMessage
                || permission == ConversationPermissions.Reply
                || permission == ConversationPermissions.React
                || permission == ConversationPermissions.UploadFile;
        }

        private static bool IsAdminPermission(string permission)
        {
            return IsMemberPermission(permission)
                || permission == ConversationPermissions.ManageMessages
                || permission == ConversationPermissions.DeleteMessage
                || permission == ConversationPermissions.AddMember
                || permission == ConversationPermissions.RemoveMember
                || permission == ConversationPermissions.ManageGroup;
        }

        private static bool IsOwnerPermission(string permission)
        {
            return IsAdminPermission(permission)
                || permission == ConversationPermissions.PromoteMember
                || permission == ConversationPermissions.DemoteMember
                || permission == ConversationPermissions.TransferOwnership
                || permission == ConversationPermissions.DeleteGroup;
        }
      
        public async Task<bool> HasPermissionAsync(Guid userId, Guid conversationId, string permission)
        {
            var member = await unitOfWork.ConversationMember.GetAsync(conversationId, userId);
            if (member == null)
            {
                return false;
            }
            return HasPermission(member.Role, permission);
        }
    }
}
