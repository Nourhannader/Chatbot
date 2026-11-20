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
    public sealed class RolePermissionService : IRolePermissionService
    {
        
        private static bool IsMessagingPermission(string permission)
        {
            return permission is ConversationPermissions.SendMessage
                   or  ConversationPermissions.Reply
                   or  ConversationPermissions.ForwardMessage
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
        public  Task<bool> HasPermissionAsync(ConversationMember member, string permission)
        {

            if (member == null)
                return Task.FromResult(false);
            if (member.LeftAt != null || member.IsBanned)
            {
                return Task.FromResult(false);
            }

            // Admin muted user
            if (IsMessagingPermission(permission) &&
                IsAdminMuted(member))
            {
                return Task.FromResult(false);
            }

            var permissions = ConversationRolePermissions.GetPermissions(member.Role);

            return Task.FromResult(permissions.Contains(permission));
        }
    }
}
