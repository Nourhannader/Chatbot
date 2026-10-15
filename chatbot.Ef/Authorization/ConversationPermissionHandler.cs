using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.Enums;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Services;
using Microsoft.AspNetCore.Authorization;

namespace chatbot.Ef.Authorization
{
    public class ConversationPermissionHandler(IRolePermissionService rolePermissionService,IUnitOfWork unitOfWork)
     : AuthorizationHandler<PermissionRequirement,ConversationResource>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement, ConversationResource resource)
        {
            if (context.User.Identity?.IsAuthenticated != true)
                return;
            var actorUserId = context.User.GetUserId();
            var actor = await unitOfWork.ConversationMember.GetActiveAsync(resource.ConversationId, actorUserId);
            if(actor is null)
            {
                return;
            }

            var hasPermission = await rolePermissionService.HasPermissionAsync(actor, requirement.Permission);
            if (!hasPermission)
                return;

            if (IsMemberTargetPermission(requirement.Permission))
            {
                var authorized = await AuthorizeMemberOperationAsync(actor, requirement.Permission, resource);
                if (!authorized)
                    return;
            }
            if (IsMessagePermission(requirement.Permission) && resource.MessageId.HasValue)
            {
                var authorized = await AuthorizeMessageOPerationAsync(actor, requirement.Permission, resource);
                if (!authorized)
                    return;
            }
            
            context.Succeed(requirement);
        }


        private async Task<bool> AuthorizeMemberOperationAsync(ConversationMember actor,string permission,ConversationResource resource)
        {
            if (!resource.TargetUserId.HasValue)
                return false;
            var targetUserId = resource.TargetUserId.Value;

            if (actor.UserId == targetUserId)
                return false;
            var target = await unitOfWork.ConversationMember.GetAsync(resource.ConversationId, targetUserId);

            if(permission == ConversationPermissions.AddMember)
            {
                return target is null || (target.LeftAt != null && !target.IsBanned);
            }
            if (target is null)
                return false;


            return permission switch
            {
                ConversationPermissions.RemoveMember => CanRemoveMember(actor, target),
                ConversationPermissions.MuteMember => CanMuteMember(actor, target),
                ConversationPermissions.UnmuteMember => CanUnMuteMember(actor, target),
                ConversationPermissions.BanMember => CanbanMember(actor, target),
                ConversationPermissions.UnbanMember => CanUnbanMember(actor, target),
                ConversationPermissions.PromoteMember => CanPromote(actor, target),
                ConversationPermissions.DemoteMember => CanDemote(actor, target),
                ConversationPermissions.TransferOwnership => CanTransferOwnership(actor, target),
                _ => false
            };
        }

        private static bool CanPromote(ConversationMember actor, ConversationMember terget)
        {
            return actor.Role == ConversationRole.Owner && terget.LeftAt == null
                && !terget.IsBanned && terget.Role == ConversationRole.Member;
        }

        private static bool CanDemote(ConversationMember actor, ConversationMember terget)
        {
            return actor.Role == ConversationRole.Owner && terget.LeftAt == null
                && !terget.IsBanned && terget.Role == ConversationRole.Admin;
        }

        private static bool CanTransferOwnership(ConversationMember actor, ConversationMember terget)
        {
            return actor.Role == ConversationRole.Owner && terget.LeftAt == null
                && !terget.IsBanned && terget.Role != ConversationRole.Owner;
        }

        private static bool CanUnbanMember(ConversationMember actor,ConversationMember target)
        {
            if (!target.IsBanned)
            {
                return false;
            }
            return ConversationRoleHierarchy.IsHigherThan(actor.Role, target.Role);
        }
        
        private static bool CanbanMember(ConversationMember actor, ConversationMember target)
        {
            return target.LeftAt ==null && !target.IsBanned
                && ConversationRoleHierarchy.IsHigherThan(actor.Role, target.Role);
        }

        private static bool CanMuteMember(ConversationMember actor, ConversationMember target)
        {
            
            return target.LeftAt == null && !target.IsBanned && !target.IsMutedByAdmin
                && ConversationRoleHierarchy.IsHigherThan(actor.Role, target.Role);
        }
       
        private static bool CanUnMuteMember(ConversationMember actor, ConversationMember target)
        {

            return target.LeftAt == null && !target.IsBanned && target.IsMutedByAdmin
                && ConversationRoleHierarchy.IsHigherThan(actor.Role, target.Role);
        }

        private static bool CanRemoveMember(ConversationMember actor, ConversationMember target)
        {
            return target.LeftAt == null && !target.IsBanned
                && ConversationRoleHierarchy.IsHigherThan(actor.Role, target.Role);
        }

        private async Task<bool> AuthorizeMessageOPerationAsync(ConversationMember actor, string permission, ConversationResource resource)
        {
            if (!resource.MessageId.HasValue)
                return false;
            var message = await unitOfWork.Messages.GetByIdAsync(resource.MessageId.Value);
            if (message is null || message.ConversationId != resource.ConversationId)
                return false;


            return permission switch
            {
                ConversationPermissions.DeleteOwnMessage =>  message.SenderId == actor.UserId,
                ConversationPermissions.ManageMessages => actor.Role is
                         ConversationRole.Admin or ConversationRole.Owner,
                _ => false
            };
        }
        
        private static bool IsMemberTargetPermission(string permission)
        {
            return permission is
                ConversationPermissions.AddMember
                or ConversationPermissions.RemoveMember
                or ConversationPermissions.PromoteMember
                or ConversationPermissions.DemoteMember
                or ConversationPermissions.MuteMember
                or ConversationPermissions.UnmuteMember
                or ConversationPermissions.BanMember
                or ConversationPermissions.UnbanMember
                or ConversationPermissions.TransferOwnership;
        }

        private static bool IsMessagePermission(string permission)
        {
            return permission is
                ConversationPermissions.DeleteOwnMessage
                or ConversationPermissions.ManageMessages;
        }

        private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out userId);
        }
    }
}
