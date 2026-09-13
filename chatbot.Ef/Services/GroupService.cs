using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Services.Helper;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class GroupService : IGroupService
    {
        private readonly IStorageService storageService;
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionService permissionService ;
        public GroupService(IUnitOfWork unitOfWork,IStorageService storageService)
        {
            this.unitOfWork = unitOfWork;
            this.storageService = storageService;
            this.permissionService = new PermissionService(unitOfWork);
        }
        public async Task AddMemberAsync(Guid conversationId, Guid currentUserId, Guid newUserId)
        {
            await permissionService.RequireAdminAsync(conversationId,currentUserId);
            var conversation =await unitOfWork.Conversations.GetByIdAsync(conversationId);

            if (conversation == null ||
                conversation.Type != ConversationType.Group)
            {
                throw new KeyNotFoundException( "Group not found.");
            }
            var existing = await unitOfWork.ConversationMember.GetAsync(conversationId,newUserId);

            if (existing != null)
            {
                if (existing.LeftAt == null)
                    throw new InvalidOperationException("User is already a member.");

                // User was member before and left
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
                        ConversationId = conversationId,
                        UserId = newUserId,
                        Role = GroupRole.Member
                    });
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<GroupDto> CreateAsync(Guid ownerId, CreateGroupDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException(
                    "Group title is required.");

            var conversation = new Conversation
            {
                Type = ConversationType.Group,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                CreatedById = ownerId
            };
            conversation.Members.Add(
                new ConversationMember
                {
                    UserId=ownerId,
                    Role=GroupRole.Owner
                });
            foreach(var userId in dto.MemberIds.Where(x => x != ownerId).Distinct())
            {
                conversation.Members.Add(
                  new ConversationMember
                 {
                  UserId = userId,
                  Role = GroupRole.Member
                 });
            }
            await unitOfWork.Conversations.AddAsync(conversation);
            await unitOfWork.SaveChangesAsync();
            return new GroupDto { 
                Title=dto.Title,
                Description=dto.Description,
                Id=conversation.Id,
                GroupPictureUrl=conversation.GroupPictureUrl,
                
                CreatedById = ownerId
            };
        }
       
        public async Task DeleteAsync(Guid conversationId, Guid userId)
        {
            await permissionService.RequireOwnerAsync(conversationId, userId);

            var conversation =
                await unitOfWork.Conversations.GetByIdAsync(conversationId);

            if (conversation == null)
                throw new KeyNotFoundException("Group not found.");

            conversation.IsDeleted = true;
            conversation.DeletedAt = DateTime.UtcNow;
            conversation.DeletedById = userId;

            unitOfWork.Conversations.Update(conversation);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task DemoteAsync(Guid conversationId, Guid currentUserId, Guid memberId)
        {
            await permissionService.RequireOwnerAsync(conversationId, currentUserId);

            var member = await permissionService.GetActiveMemberAsync(conversationId, memberId);

            if (member.Role != GroupRole.Admin)
                throw new InvalidOperationException("Only admins can be demoted.");

            member.Role = GroupRole.Member;

           unitOfWork.ConversationMember.Update(member);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task LeaveAsync(Guid conversationId, Guid userId)
        {
            var member = await permissionService.GetActiveMemberAsync(conversationId, userId);

            if (member.Role == GroupRole.Owner)
            {
                var members =
                    await unitOfWork.ConversationMember.GetActiveMembersAsync(conversationId);

                var newOwner = members
                    .Where(x => x.UserId != userId)
                    .OrderByDescending(x => x.Role == GroupRole.Admin)
                    .ThenBy(x => x.JoinedAt)
                    .FirstOrDefault();

                if (newOwner == null)
                {
                    member.LeftAt = DateTime.UtcNow;

                    unitOfWork.ConversationMember.Update(member);

                    await unitOfWork.SaveChangesAsync();

                    return;
                }

                newOwner.Role = GroupRole.Owner;
                member.LeftAt = DateTime.UtcNow;

                unitOfWork.ConversationMember.Update(newOwner);
                unitOfWork.ConversationMember.Update(member);
            }
            else
            {
                member.LeftAt = DateTime.UtcNow;

                unitOfWork.ConversationMember.Update(member);
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task PromoteAsync(Guid conversationId, Guid currentUserId, Guid memberId)
        {
            await permissionService.RequireOwnerAsync(conversationId, currentUserId);

            var member = await permissionService.GetActiveMemberAsync(conversationId, memberId);

            if (member.Role != GroupRole.Member)
                throw new InvalidOperationException(
                    "Only members can be promoted.");

            member.Role = GroupRole.Admin;

            unitOfWork.ConversationMember.Update(member);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(Guid conversationId, Guid currentUserId, Guid memberId)
        {
            var currentUser = await permissionService.GetActiveMemberAsync(conversationId, currentUserId);

            var target = await permissionService.GetActiveMemberAsync(conversationId, memberId);

            if (target.Role == GroupRole.Owner)
                throw new InvalidOperationException("Owner cannot be removed.");

            if (currentUser.Role == GroupRole.Member)
                throw new UnauthorizedAccessException();

            if (currentUser.Role == GroupRole.Admin &&
                target.Role == GroupRole.Admin)
            {
                throw new UnauthorizedAccessException("Admin cannot remove another admin.");
            }

            target.LeftAt = DateTime.UtcNow;

            unitOfWork.ConversationMember.Update(target);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task TransferOwnershipAsync(Guid conversationId, Guid ownerId, Guid newOwnerId)
        {
            var owner = await permissionService.RequireOwnerAsync(conversationId, ownerId);

            var newOwner = await permissionService.GetActiveMemberAsync(conversationId, newOwnerId);

            if (newOwner.UserId == owner.UserId)
                throw new InvalidOperationException("User is already the owner.");

            owner.Role = GroupRole.Admin;
            newOwner.Role = GroupRole.Owner;

            unitOfWork.ConversationMember.Update(owner);
            unitOfWork.ConversationMember.Update(newOwner);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid conversationId, Guid userId, UpdateGroupDto dto)
        {
            await permissionService.RequireAdminAsync(conversationId, userId);

            var conversation = await unitOfWork.Conversations.GetByIdAsync(conversationId);

            if (conversation == null ||
                conversation.Type != ConversationType.Group)
            {
                throw new KeyNotFoundException(
                    "Group not found.");
            }

            if (dto.Title != null)
                conversation.Title = dto.Title.Trim();

            if (dto.Description != null)
                conversation.Description = dto.Description.Trim();

            if (dto.Image != null)
            {
                var result =
                    await storageService.UploadAsync(dto.Image, "groups", userId,conversationId);//return here

                conversation.GroupPictureUrl = result.FileUrl;
            }

            unitOfWork.Conversations.Update(conversation);

            await unitOfWork.SaveChangesAsync();
        }
    }
}
