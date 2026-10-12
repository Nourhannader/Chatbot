using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Authorization
{
    public class RolePermissionService(IUnitOfWork unitOfWork) : IRolePermissionService
    {
        
        private static bool IsMessagingPermission(string permission)
        {
            return permission is ConversationPermissions.SendMessage
                   or  ConversationPermissions.Reply
                   or  ConversationPermissions.UploadFile;
        }

        private static bool IsAdminMuted(ConversationMember member)
        {
            if (!member.IsMutedByAdmin)
                return false;

            // Indefinite mute
            if (!member.MutedUntil.HasValue)
                return true;

            return member.MutedUntil.Value >
                   DateTime.UtcNow;
        }
        public async Task<bool> HasPermissionAsync(ConversationMember member, string permission)
        {

            if (member == null)
                return false;
            if (member.LeftAt != null || member.IsBanned)
            {
                return false;
            }

            // Admin muted user
            if (IsMessagingPermission(permission) &&
                IsAdminMuted(member))
            {
                return false;
            }

            if (!ConversationRolePermissions.Permissions.TryGetValue(member.Role, out var permissions))
            {
                return false;
            }

            return permissions.Contains(permission);
        }
    }
}
