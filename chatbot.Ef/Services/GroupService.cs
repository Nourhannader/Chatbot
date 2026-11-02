using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Outbox;
using chatbot.Ef.Services.Helper;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class GroupService(IUnitOfWork unitOfWork, IStorageService storageService, IConversationAuthorizationService authorizationService,
        IMapper mapper,TimeProvider timeProvider,OutboxMessageFactory outboxMessageFactory,ICurrentUser currentUser) : IGroupService
    {
        public async Task AddMemberAsync(ClaimsPrincipal user,Guid conversationId,Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, userId, ConversationPermissions.AddMember);

            var group = await unitOfWork.Groups
                    .GetByIdAsync(conversationId)
                ?? throw new NotFoundException("Group not found.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var existing = await unitOfWork.ConversationMember
                    .GetAsync(conversationId, userId);

            ConversationMember member;

            if (existing is not null)
            {
                if (existing.IsBanned)
                {
                    throw new ForbiddenException("Banned user cannot be added.");
                }

                if (existing.LeftAt is null)
                {
                    throw new ConflictException("User is already a member.");
                }

                existing.LeftAt = null;
                existing.JoinedAt = now;
                existing.Role = ConversationRole.Member;
                member = existing;
            }
            else
            {
                member =
                    new ConversationMember
                    {
                        Id = Guid.NewGuid(),
                        ConversationId = conversationId,
                        UserId = userId,
                        Role = ConversationRole.Member,
                        JoinedAt = now
                    };

                await unitOfWork.ConversationMember.AddAsync(member);
            }

            await AddOutboxEventAsync(new MemberAddedEvent(conversationId, userId, currentUser.UserId, ConversationRole.Member, now));


            await unitOfWork.SaveChangesAsync();
        }

        public async Task<ConversationDetailsDto> CreateAsync(ClaimsPrincipal user, CreateGroupDto dto)
        {
            var ownerId = currentUser.UserId;

            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new BadRequestException("Group title is required.");

            var memberIds = dto.MemberIds?
                .Where(id => id != Guid.Empty && id != ownerId)
                .Distinct().ToList() ?? [];

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Group,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                CreatedById = ownerId,
                CreatedAt = now
            };

            await unitOfWork.Conversations.AddAsync(conversation);

            await unitOfWork.ConversationMember
                .AddAsync(
                    new ConversationMember
                    {
                        Id = Guid.NewGuid(),
                        ConversationId = conversation.Id,
                        UserId = ownerId,
                        Role = ConversationRole.Owner,
                        JoinedAt = now
                    });

            foreach (var memberId in memberIds)
            {

                await unitOfWork.ConversationMember
                    .AddAsync(
                        new ConversationMember
                        {
                            Id = Guid.NewGuid(),
                            ConversationId = conversation.Id,
                            UserId = memberId,
                            Role = ConversationRole.Member,
                            JoinedAt = now
                        });
            }


            //create conversation event
            await AddOutboxEventAsync( new ConversationCreatedEvent( conversation.Id, ownerId, ConversationType.Group, now));
            foreach (var memberId in memberIds)
            {
                await AddOutboxEventAsync(new MemberAddedEvent(conversation.Id, memberId, ownerId, ConversationRole.Member, now));
            }

            await unitOfWork.SaveChangesAsync();
            var result = mapper.Map<ConversationDetailsDto>(conversation);
            result.CurrentUserRole = ConversationRole.Owner;
            return result;
        }
       
        public async Task DeleteAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(user,conversationId,ConversationPermissions.DeleteGroup);

            var group = await unitOfWork.Groups
                .GetByIdAsync(conversationId) ?? throw new NotFoundException("Group not found.");

            if (group.IsDeleted)
                throw new BadRequestException("Group is already deleted.");

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var userId = currentUser.UserId;

            group.IsDeleted = true;
            group.DeletedAt = now;
            group.DeletedById = userId;

            await AddOutboxEventAsync(new ConversationDeletedEvent( conversationId,userId, now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task DemoteAsync(ClaimsPrincipal user, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, memberId,ConversationPermissions.DemoteMember);

            var member = await unitOfWork.ConversationMember
                .GetActiveAsync(conversationId, memberId)
                ?? throw new NotFoundException("Member not found.");

            if (member.Role != ConversationRole.Admin)
                throw new BadRequestException("Only admins can be demoted.");

            var oldRole = member.Role;
            var now = timeProvider.GetUtcNow().UtcDateTime;

            member.Role = ConversationRole.Member;

            await AddOutboxEventAsync(new MemberRoleChangedEvent(conversationId, memberId, oldRole, ConversationRole.Member,currentUser.UserId, now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task LeaveAsync(ClaimsPrincipal user,Guid conversationId)
        {
            var userId = currentUser.UserId;
            var member = await unitOfWork.ConversationMember
           .GetActiveAsync(conversationId, userId)
           ?? throw new NotFoundException( "You are not an active member of this group.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            if (member.Role == ConversationRole.Owner)
            {
                var members =
                    await unitOfWork.ConversationMember.GetActiveMembersAsync(conversationId);

                var newOwner = members
                    .Where(x => x.UserId != userId)
                    .OrderByDescending(x => x.Role == ConversationRole.Admin)
                    .ThenBy(x => x.JoinedAt)
                    .FirstOrDefault();

                if (newOwner is not null)
                {
                    var oldRole = newOwner.Role;

                    newOwner.Role = ConversationRole.Owner;

                    await AddOutboxEventAsync(new MemberRoleChangedEvent(conversationId, newOwner.UserId,oldRole, ConversationRole.Owner, userId, now));
                }

                else
                {
                    // No other active member exists.
                    // Mark the group as deleted to avoid an ownerless active group.
                    var group = await unitOfWork.Groups.GetByIdAsync(conversationId)
                        ?? throw new NotFoundException("Group not found.");

                    group.IsDeleted = true;
                    group.DeletedAt = now;
                    group.DeletedById = userId;

                    await AddOutboxEventAsync( new ConversationDeletedEvent(conversationId, userId, now));
                }
            }
            member.LeftAt = now;

            await AddOutboxEventAsync( new MemberRemovedEvent(conversationId,userId, null,MemberRemovalType.Left,now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task PromoteAsync(ClaimsPrincipal user, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, memberId, ConversationPermissions.PromoteMember);

            var member = await unitOfWork.ConversationMember
                    .GetActiveAsync(conversationId, memberId)
                ?? throw new NotFoundException("Member not found.");

            if (member.Role != ConversationRole.Member)
                throw new BadRequestException("Only regular members can be promoted.");

            var oldRole = member.Role;
            var now = timeProvider.GetUtcNow().UtcDateTime;

            member.Role = ConversationRole.Admin;
            await AddOutboxEventAsync(new MemberRoleChangedEvent(conversationId, memberId, oldRole,ConversationRole.Admin, currentUser.UserId, now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, memberId, ConversationPermissions.RemoveMember);

            var member = await unitOfWork.ConversationMember
            .GetActiveAsync(conversationId, memberId) ?? throw new NotFoundException("Member not found.");

            if (member.Role == ConversationRole.Owner)
                throw new BadRequestException( "The group owner cannot be removed. Transfer ownership first.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            member.LeftAt = now;

            await AddOutboxEventAsync(new MemberRemovedEvent(conversationId, memberId, currentUser.UserId, MemberRemovalType.Removed, now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task TransferOwnershipAsync(ClaimsPrincipal user, Guid conversationId, Guid newOwnerId)
        {
            var currentOwnerId = currentUser.UserId;

            if (currentOwnerId == newOwnerId)
                throw new BadRequestException("You are already the group owner.");

            var oldOwner = await unitOfWork.ConversationMember
                .GetActiveAsync(conversationId, currentOwnerId) ?? throw new NotFoundException("Current owner not found.");

            if (oldOwner.Role != ConversationRole.Owner)
                throw new ForbiddenException("Only the owner can transfer ownership.");

            await authorizationService.AuthorizeMemberAsync(user, conversationId, newOwnerId,ConversationPermissions.TransferOwnership);

            var newOwner = await unitOfWork.ConversationMember
                .GetActiveAsync(conversationId, newOwnerId)
                ?? throw new NotFoundException("New owner not found.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var oldRole = oldOwner.Role;
            var newRole = newOwner.Role;

            oldOwner.Role = ConversationRole.Admin;
            newOwner.Role = ConversationRole.Owner;

            await AddOutboxEventAsync( new MemberRoleChangedEvent(conversationId, currentOwnerId, oldRole, ConversationRole.Admin, currentOwnerId,now));

            await AddOutboxEventAsync(new MemberRoleChangedEvent(conversationId, newOwnerId, newRole, ConversationRole.Owner, currentOwnerId, now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<ConversationDetailsDto> UpdateAsync(ClaimsPrincipal user,Guid conversationId, UpdateGroupDto dto)
        {
            await authorizationService.AuthorizeAsync( user, conversationId, ConversationPermissions.ManageGroup);

            var group = await unitOfWork.Groups
                .GetByIdAsync(conversationId)
                ?? throw new NotFoundException("Group not found.");

            if (dto.Title is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Title))
                    throw new BadRequestException("Group title cannot be empty.");

                group.Title = dto.Title.Trim();
            }

            if (dto.Description is not null)
                group.Description = dto.Description.Trim();

            var now = timeProvider.GetUtcNow().UtcDateTime;

            await AddOutboxEventAsync(new ConversationUpdatedEvent( conversationId,currentUser.UserId,group.Title,group.Description,now));

            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ConversationDetailsDto>(group);
        }
  
        public async Task BanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId, BanMemberDto dto)
        {
            await authorizationService.AuthorizeMemberAsync(user,conversationId,userId,ConversationPermissions.BanMember);

            var member = await unitOfWork.ConversationMember
                .GetActiveAsync(conversationId, userId) ?? throw new NotFoundException("Member not found.");

            if (member.Role == ConversationRole.Owner)
                throw new BadRequestException("The group owner cannot be banned.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            member.IsBanned = true;
            member.BannedAt = now;
            member.BannedById = user.GetUserId();
            member.BanReason = dto.Reason;

            // Banning also removes the user from active membership.
            member.LeftAt = now;

            await AddOutboxEventAsync(new MemberRemovedEvent(conversationId, userId,currentUser.UserId, MemberRemovalType.Banned, now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task UnBanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, userId,ConversationPermissions.UnbanMember);

            var member = await unitOfWork.ConversationMember
                .GetAsync(conversationId, userId) ?? throw new NotFoundException("Member not found.");

            if (!member.IsBanned)
                throw new BadRequestException("Member is not banned.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            member.IsBanned = false;
            member.BannedAt = null;
            member.BannedById = null;
            member.BanReason = null;

            // Unbanning does not automatically rejoin the user.
            // LeftAt remains unchanged.

            await AddOutboxEventAsync(new MemberUnbannedEvent(conversationId, userId, currentUser.UserId, now));

            await unitOfWork.SaveChangesAsync();
        }

        public async Task MuteMemberAsync(ClaimsPrincipal user,Guid conversationId,Guid userId, MuteConversationDto dto)
        {
            await authorizationService.AuthorizeMemberAsync(user,conversationId, userId, ConversationPermissions.MuteMember);

            var member = await unitOfWork.ConversationMember
                .GetActiveAsync(conversationId, userId) ?? throw new NotFoundException("Member not found.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            if (dto.MuteUntil.HasValue && dto.MuteUntil.Value <= now)
            {
                throw new BadRequestException(
                    "MuteUntil must be in the future.");
            }

            member.IsMutedByAdmin = true;
            member.MutedUntil = dto.MuteUntil;
            member.MutedById = currentUser.UserId;

            await AddOutboxEventAsync(new MemberMutedEvent(conversationId,userId,currentUser.UserId,dto.MuteUntil,now));

            await unitOfWork.SaveChangesAsync();
        }
        public async Task UnMuteMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user,conversationId, userId,ConversationPermissions.UnmuteMember);

            var member = await unitOfWork.ConversationMember
                .GetActiveAsync(conversationId, userId) ?? throw new NotFoundException("Member not found.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            member.IsMutedByAdmin = false;
            member.MutedUntil = null;
            member.MutedById = null;

            await AddOutboxEventAsync(new MemberUnmutedEvent(conversationId,userId,currentUser.UserId,now));

            await unitOfWork.SaveChangesAsync();
        }

        private async Task AddOutboxEventAsync<TEvent>(TEvent domainEvent) where TEvent : IEvent
        {
            var outboxMessage = outboxMessageFactory.Create(domainEvent);

            await unitOfWork.OutboxMessages.AddAsync(outboxMessage);
        }

    }
}
