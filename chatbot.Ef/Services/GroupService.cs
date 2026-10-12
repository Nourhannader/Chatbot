using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
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
        private readonly IConversationAuthorizationService authorizationService ;
        public GroupService(IUnitOfWork unitOfWork,IStorageService storageService,IConversationAuthorizationService authorizationService)
        {
            this.unitOfWork = unitOfWork;
            this.storageService = storageService;
            this.authorizationService = authorizationService;
        }
        public async Task AddMemberAsync(ClaimsPrincipal User,Guid conversationId, Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(User, conversationId,userId, ConversationPermissions.AddMember);
            var conversation = await unitOfWork.Conversations.GetGroupWithMembersAsync(conversationId);

            if (conversation == null || conversation.Type != ConversationType.Group)
            {
                throw new KeyNotFoundException("Group not found.");
            }
            var member = await unitOfWork.ConversationMember.GetAsync(conversationId, userId);
            if (member != null)
            {
                if (member.LeftAt == null)
                    throw new ConflictException("User is already a member");
                if (member.IsBanned)
                    throw new ForbiddenException("Banned User cann't be added.");
                member.LeftAt = null;
                member.JoinedAt = DateTime.UtcNow;
                member.Role = ConversationRole.Member;
                unitOfWork.ConversationMember.Update(member);
            }
            else
            {
                await unitOfWork.ConversationMember.AddAsync(new ConversationMember
                {
                    Id=Guid.NewGuid(),
                    ConversationId=conversationId,
                    UserId=userId,
                    Role=ConversationRole.Member,
                    JoinedAt=DateTime.UtcNow
                });
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<Guid> CreateAsync(Guid ownerId, CreateGroupDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Group title is required.");

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Group,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                CreatedById = ownerId,
                CreatedAt = DateTime.UtcNow
            };
            
            var member= new ConversationMember
                {
                    Id=Guid.NewGuid(),
                    ConversationId=conversation.Id,
                    UserId=ownerId,
                    Role=ConversationRole.Owner,
                    JoinedAt=DateTime.UtcNow
                };
            conversation.Members.Add(member);
            
            await unitOfWork.Conversations.AddAsync(conversation);
            await unitOfWork.SaveChangesAsync();
            return conversation.Id;
        }
       
        public async Task DeleteAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.DeleteGroup);

            var conversation =
                await unitOfWork.Conversations.GetByIdAsync(conversationId);

            if (conversation == null || conversation.Type != ConversationType.Group)
                throw new KeyNotFoundException("Group not found.");

            conversation.IsDeleted = true;
            conversation.DeletedAt = DateTime.UtcNow;

            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            conversation.DeletedById = userId;

            unitOfWork.Conversations.Update(conversation);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task DemoteAsync(ClaimsPrincipal User, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(User, conversationId,memberId ,ConversationPermissions.DemoteMember);

            var member = await unitOfWork.ConversationMember.GetActiveMemberAsync(conversationId, memberId);
            if (member == null)
            {
                throw new NotFoundException("Member not found.");
            }

            member.Role = ConversationRole.Member;

            unitOfWork.ConversationMember.Update(member);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task LeaveAsync(Guid conversationId, Guid userId)
        {
            var member = await unitOfWork.ConversationMember.GetAsync(conversationId, userId);

            if (member == null)
            {
                throw new NotFoundException("You are not a member of this group.");
            }

            if (member.Role == ConversationRole.Owner)
            {
                var members =
                    await unitOfWork.ConversationMember.GetActiveMembersAsync(conversationId);

                var newOwner = members
                    .Where(x => x.UserId != userId)
                    .OrderByDescending(x => x.Role == ConversationRole.Admin)
                    .ThenBy(x => x.JoinedAt)
                    .FirstOrDefault();

                if (newOwner == null)
                {
                    member.LeftAt = DateTime.UtcNow;

                    unitOfWork.ConversationMember.Update(member);

                    await unitOfWork.SaveChangesAsync();

                    return;
                }

                newOwner.Role = ConversationRole.Owner;
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

        public async Task PromoteAsync(ClaimsPrincipal User, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(User, conversationId, memberId, ConversationPermissions.PromoteMember);

            var member = await unitOfWork.ConversationMember.GetActiveMemberAsync(conversationId, memberId);
            if (member == null)
            {
                throw new NotFoundException("Member not found.");
            }
           

            member.Role = ConversationRole.Admin;

            unitOfWork.ConversationMember.Update(member);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(ClaimsPrincipal User, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(User, conversationId,memberId, ConversationPermissions.RemoveMember);

            var target = await unitOfWork.ConversationMember.GetActiveMemberAsync(conversationId, memberId);
            if (target == null)
            {
                throw new NotFoundException("Member not found.");
            }


            target.LeftAt = DateTime.UtcNow;

            unitOfWork.ConversationMember.Update(target);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task TransferOwnershipAsync(ClaimsPrincipal user, Guid conversationId, Guid newOwnerId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, newOwnerId, ConversationPermissions.TransferOwnership);

            var currentOwnerId = user.GetUserId();

            var owner = await unitOfWork.ConversationMember.GetActiveMemberAsync(conversationId, currentOwnerId);
            var newOwner = await unitOfWork.ConversationMember.GetActiveMemberAsync(conversationId, newOwnerId);

            if (owner == null || owner.Role != ConversationRole.Owner)
                throw new ForbiddenException("Only the owner can transfer ownership.");

            if (newOwner == null)
            {
                throw new NotFoundException("New owner is not an active member.");
            }

            owner.Role = ConversationRole.Admin;
            newOwner.Role = ConversationRole.Owner;


            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClaimsPrincipal User,Guid conversationId, UpdateGroupDto dto)
        {
            await authorizationService.AuthorizeAsync(User, conversationId, ConversationPermissions.ManageGroup);
            var conversation = await unitOfWork.Conversations.GetByIdAsync(conversationId);

            if (conversation == null ||
                conversation.Type != ConversationType.Group)
            {
                throw new KeyNotFoundException("Group not found.");
            }

            if (dto.Title != null)
            {

                conversation.Title = dto.Title.Trim();
            }

            if (dto.Description != null)
            {
                conversation.Description = dto.Description.Trim();
            }
            var uploadedBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (dto.Image != null)
            {
                var result =
                    await storageService.UploadConversationImageAsync(dto.Image,conversationId,uploadedBy);//return here

                conversation.ImageId = result.FileId;
            }

            unitOfWork.Conversations.Update(conversation);

            await unitOfWork.SaveChangesAsync();
        }
  
        public async Task BanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, userId,ConversationPermissions.BanMember);
            var member = await unitOfWork.ConversationMember.GetAsync(conversationId, userId);
            if (member == null || member.LeftAt !=null)
            {
                throw new NotFoundException("Member not found.");
            }

            member.IsBanned = true;
            member.BannedAt = DateTime.UtcNow;
            member.BannedById = user.GetUserId();
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UnBanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId,userId, ConversationPermissions.BanMember);
            var member = await unitOfWork.ConversationMember.GetAsync(conversationId, userId);
            if (member == null)
            {
                throw new NotFoundException("Member not found.");
            }
            if (!member.IsBanned)
                throw new BadRequestException("Member is not banned.");

            member.IsBanned = false;
            member.BannedAt = null;
            member.BannedById = null;
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MuteMemberAsync(ClaimsPrincipal user,Guid conversationId,Guid userId,DateTime? until=null)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId,userId, ConversationPermissions.MuteMember);
            var member = await unitOfWork.ConversationMember.GetActiveMemberAsync(conversationId, userId);
            if (member == null)
            {
                throw new NotFoundException("Member not found.");
            }
            member.IsMutedByAdmin = true;
            member.MutedUntil = until;
            member.MutedById = user.GetUserId();
           await unitOfWork.SaveChangesAsync();
        }
        public async Task UnMuteMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, userId,ConversationPermissions.UnmuteMember);
            var member = await unitOfWork.ConversationMember.GetActiveMemberAsync(conversationId, userId);
            if (member == null)
            {
                throw new NotFoundException("Member not found.");
            }
            

            member.IsMutedByAdmin = false;
            member.MutedUntil = null;
            member.MutedById = null;
            await unitOfWork.SaveChangesAsync();
        }
   
        
    }
}
