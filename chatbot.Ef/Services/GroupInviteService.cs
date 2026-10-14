using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.InviteGroup;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Services.Helper;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public sealed class GroupInviteService(IUnitOfWork unitOfWork,
    IConversationAuthorizationService authorizationService,
    IMapper mapper,
    IChatRealtimeService realtimeService,
    TimeProvider timeProvider) : IGroupInviteService
    {
        public async Task<GroupInviteDto> GenerateAsync(ClaimsPrincipal user, Guid conversationId, CreateGroupInviteDto dto)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.ManageInvites);

            var conversation = await unitOfWork.Conversations
                    .GetByIdAsync(conversationId) ?? throw new NotFoundException("Conversation not found.");

            if (conversation.Type != ConversationType.Group)
            {
                throw new BadRequestException("Invites can only be created for groups.");
            }

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var expiresAt = dto.ExpiresAt ?? now.AddDays(7);

            if (expiresAt <= now)
            {
                throw new BadRequestException("Expiration date must be in the future.");
            }

            var maxUses = dto.MaxUses ?? 100;

            if (maxUses <= 0)
            {
                throw new BadRequestException("Max uses must be greater than zero.");
            }

            var invite = new GroupInvite
            {
                Id = Guid.NewGuid(),

                ConversationId =conversationId,

                Code = GenerateSecureCode(),

                CreatedAt = now,

                CreatedById = user.GetUserId(),

                ExpiresAt = expiresAt,

                MaxUses = maxUses,

                UsedCount = 0,

                IsActive = true
            };

            await unitOfWork.GroupInvite.AddAsync(invite);

            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GroupInviteDto>(invite);
        }

        public async Task<List<GroupInviteDto>> GetActiveAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.ManageInvites);

            var invites = await unitOfWork.GroupInvite.GetActiveInvitesAsync(conversationId);

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var activeInvites = invites.Where(x =>
                        x.ExpiresAt > now &&
                        x.UsedCount < x.MaxUses)
                    .ToList();

            return mapper.Map<List<GroupInviteDto>>(activeInvites);
        }

        public async Task<GroupInvitePreviewDto> GetPreviewAsync(ClaimsPrincipal user, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new BadRequestException("Invite code is required.");
            }

            var invite = await unitOfWork.GroupInvite
                    .GetByCodeAsync(code)
                ?? throw new NotFoundException("Invite not found.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var valid = IsInviteValid(invite, now);

            var userId = user.GetUserId();

            var member = await unitOfWork.ConversationMember
                    .GetAsync(invite.ConversationId, userId);

            var canJoin = valid && !invite.Conversation.IsDeleted &&
                (
                    member is null ||
                    (
                        member.LeftAt != null &&
                        !member.IsBanned
                    )
                );

            return new GroupInvitePreviewDto
            {
                ConversationId = invite.ConversationId,

                GroupName = invite.Conversation.Title ?? "Group",

                Description = invite.Conversation.Description,

                ImageUrl = invite.Conversation.Image?.Path,

                MemberCount = await unitOfWork.ConversationMember.CountActiveAsync(invite.ConversationId),

                ExpiresAt = invite.ExpiresAt,

                IsExpired = invite.ExpiresAt <= now,

                CanJoin = canJoin
            };
        }

        public async Task<ConversationDetailsDto> JoinAsync(ClaimsPrincipal user, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new BadRequestException("Invite code is required.");
            }

            var userId = user.GetUserId();

            var invite = await unitOfWork.GroupInvite
                    .GetByCodeAsync(code)
                ?? throw new NotFoundException("Invite not found.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            ValidateInvite(invite, now);

            if (invite.Conversation.Type != ConversationType.Group)
            {
                throw new BadRequestException("Invalid group invite.");
            }

            if (invite.Conversation.IsDeleted)
            {
                throw new NotFoundException("Group not found.");
            }

            var member =
                await unitOfWork.ConversationMember
                    .GetAsync(invite.ConversationId, userId);

            if (member is not null)
            {
                if (member.IsBanned)
                {
                    throw new ForbiddenException("You are banned from this group.");
                }

                if (member.LeftAt is null)
                {
                    throw new ConflictException("You are already a member of this group.");
                }

                // Rejoin old membership row
                member.LeftAt = null;
                member.JoinedAt = now;
                member.Role = ConversationRole.Member;

                // Clear stale moderation state if your domain policy requires it.
                member.IsMutedByAdmin = false;
                member.MutedUntil = null;
                member.MutedById = null;
            }
            else
            {
                member = new ConversationMember
                {
                    Id = Guid.NewGuid(),

                    ConversationId = invite.ConversationId,

                    UserId = userId,

                    Role = ConversationRole.Member,

                    JoinedAt = now
                };

                await unitOfWork.ConversationMember.AddAsync(member);
            }

            invite.UsedCount++;

            if (invite.UsedCount >= invite.MaxUses)
            {
                invite.IsActive = false;
            }

            await unitOfWork.SaveChangesAsync();

            // Realtime AFTER successful DB commit.
            var memberDto = mapper.Map<ConversationMemberDto>(member);

            await realtimeService.MemberAddedAsync(invite.ConversationId, memberDto);

            var result = mapper.Map<ConversationDetailsDto>(invite.Conversation);

            result.CurrentUserRole = ConversationRole.Member;

            return result;
        }

        public async Task RevokeAsync(ClaimsPrincipal user, Guid conversationId, Guid inviteId)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.ManageInvites);

            var invite = await unitOfWork.GroupInvite.GetByIdAsync(inviteId)
                ?? throw new NotFoundException("Invite not found.");

            if (invite.ConversationId != conversationId)
            {
                throw new NotFoundException("Invite not found.");
            }

            if (!invite.IsActive)
                return;

            invite.IsActive = false;

            invite.RevokedAt = timeProvider.GetUtcNow().UtcDateTime;

            invite.RevokedById = user.GetUserId();

            await unitOfWork.SaveChangesAsync();
        }

        private static string GenerateSecureCode()
        {
            return Convert.ToHexString(
                RandomNumberGenerator.GetBytes(16))
                .ToLowerInvariant();
        }
        private static bool IsInviteValid(GroupInvite invite, DateTime now)
        {
            return
                invite.IsActive &&
                invite.ExpiresAt > now &&
                invite.UsedCount < invite.MaxUses;
        }

        private static void ValidateInvite(GroupInvite invite, DateTime now)
        {
            if (!invite.IsActive)
            {
                throw new BadRequestException( "Invite is no longer active.");
            }

            if (invite.ExpiresAt <= now)
            {
                throw new BadRequestException("Invite has expired.");
            }

            if (invite.UsedCount >= invite.MaxUses)
            {
                throw new BadRequestException("Invite usage limit has been reached.");
            }
        }
    }
}
