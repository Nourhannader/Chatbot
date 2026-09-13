using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Services.Helper;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class GroupInviteService : IGroupInviteService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionService permissionService;
        public GroupInviteService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.permissionService = new PermissionService(unitOfWork);
        }
        public async Task<string> GenerateAsync(Guid conversationId, Guid userId)
        {
            var member = await permissionService.GetActiveMemberAsync(conversationId, userId);

            if (member.Role == GroupRole.Member)
                throw new UnauthorizedAccessException("Only admins can create invite links.");

            var invite = new GroupInvite
            {
                ConversationId = conversationId,
                Code = Convert.ToHexString(
                    RandomNumberGenerator.GetBytes(16)),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                MaxUses = 100
            };

            await unitOfWork.GroupInvite.AddAsync(invite);

            await unitOfWork.SaveChangesAsync();

            return invite.Code;
        }

        public async Task JoinAsync(string code, Guid userId)
        {
            var invite =await unitOfWork.GroupInvite.GetByCodeAsync(code);

            if (invite == null || !invite.IsActive)
                throw new InvalidOperationException("Invalid invite link.");

            if (invite.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Invite link has expired.");

            if (invite.UsedCount >= invite.MaxUses)
                throw new InvalidOperationException("Invite usage limit reached.");

            var existing =await unitOfWork.ConversationMember.GetAsync(invite.ConversationId, userId);

            if (existing != null &&existing.LeftAt == null)
            {
                throw new InvalidOperationException("User is already a member.");
            }

            if (existing != null)
            {
                existing.LeftAt = null;
                existing.JoinedAt = DateTime.UtcNow;
                existing.Role = GroupRole.Member;

                unitOfWork.ConversationMember.Update(existing);
            }
            else
            {
                await unitOfWork.ConversationMember.AddAsync(
                    new ConversationMember
                    {
                        ConversationId = invite.ConversationId,
                        UserId = userId,
                        Role = GroupRole.Member
                    });
            }

            invite.UsedCount++;

            unitOfWork.GroupInvite.Update(invite);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task RevokeAsync(Guid inviteId, Guid userId)
        {
            var invite = await unitOfWork.GroupInvite.GetByIdAsync(inviteId);

            if (invite == null)
                throw new KeyNotFoundException("Invite not found.");

            var member = await unitOfWork.ConversationMember.GetAsync(invite.ConversationId, userId);

            if (member == null || member.LeftAt != null)
                throw new UnauthorizedAccessException("You are not an active member of this group.");

            if (member.Role != GroupRole.Owner &&
                member.Role != GroupRole.Admin)
            {
                throw new UnauthorizedAccessException("Only admins or the owner can revoke an invite.");
            }

            if (!invite.IsActive)
                return;

            invite.IsActive = false;

            unitOfWork.GroupInvite.Update(invite);

            await unitOfWork.SaveChangesAsync();
        }
    }
}
