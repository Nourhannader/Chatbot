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
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Services.Helper;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class GroupService(IUnitOfWork unitOfWork, IStorageService storageService, IConversationAuthorizationService authorizationService,
        IMapper mapper,TimeProvider timeProvider) : IGroupService
    {
        public async Task AddMemberAsync(ClaimsPrincipal user,Guid conversationId,Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, userId, ConversationPermissions.AddMember);

            var group = await unitOfWork.Groups
                    .GetByIdAsync(conversationId)
                ?? throw new NotFoundException("Group not found.");

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
                existing.JoinedAt = DateTime.UtcNow;
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
                        JoinedAt = DateTime.UtcNow
                    };

                await unitOfWork.ConversationMember.AddAsync(member);
            }

            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<ConversationMemberDto>(member);
        }

        public async Task<ConversationDetailsDto> CreateAsync(ClaimsPrincipal user, CreateGroupDto dto)
        {
            var ownerId = user.GetUserId();

            var now = timeProvider.GetUtcNow().UtcDateTime;

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

            foreach (var memberId in dto.MemberIds
                .Where(x => x != ownerId)
                .Distinct())
            {
                if (memberId == ownerId)
                    continue;

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

            //// Image upload with compensation
            //StoredFile? imageFile = null;

            //try
            //{
            //    if (dto.Image is not null)
            //    {
            //        var upload = await storageService.UploadAsync(dto.Image, $"conversations/{conversation.Id:N}");
            //        imageFile = CreateStoredFile(dto.Image, upload);
            //        await unitOfWork.StoredFiles.AddAsync(imageFile);
            //        conversation.ImageId = imageFile.Id;
            //        conversation.Image = imageFile;
            //    }

            //    await unitOfWork.SaveChangesAsync();
            //}
            //catch
            //{
            //    if (imageFile is not null)
            //    {
            //        await TryDeleteAsync(imageFile.StorageKey);
            //    }

            //    throw;
            //}

            var result = mapper.Map<ConversationDetailsDto>(conversation);
            result.CurrentUserRole = ConversationRole.Owner;
            return result;
        }
       
        public async Task DeleteAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.DeleteGroup);

            var group = await unitOfWork.Groups.GetByIdAsync(conversationId)
                 ?? throw new NotFoundException("Group not found.");

            group.IsDeleted = true;
            group.DeletedAt = DateTime.UtcNow;

            group.DeletedById = user.GetUserId();

            await unitOfWork.SaveChangesAsync();
        }

        public async Task DemoteAsync(ClaimsPrincipal user, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId,memberId, ConversationPermissions.DemoteMember);

            var member = await unitOfWork.ConversationMember
                    .GetActiveAsync(conversationId, memberId)
                ?? throw new NotFoundException("Member not found.");

            member.Role = ConversationRole.Member;

            await unitOfWork.SaveChangesAsync();
        }

        public async Task LeaveAsync(ClaimsPrincipal user,Guid conversationId)
        {
            var userId = user.GetUserId();
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

        public async Task PromoteAsync(ClaimsPrincipal user, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, memberId, ConversationPermissions.PromoteMember);

            var member = await unitOfWork.ConversationMember
                    .GetActiveAsync(conversationId, memberId)
                ?? throw new NotFoundException("Member not found.");

            member.Role = ConversationRole.Admin;

            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid memberId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, memberId, ConversationPermissions.RemoveMember);

            var member = await unitOfWork.ConversationMember
                    .GetActiveAsync(conversationId, memberId)
                ?? throw new NotFoundException("Member not found.");

            member.LeftAt = timeProvider.GetUtcNow().UtcDateTime;

            await unitOfWork.SaveChangesAsync();
        }

        public async Task TransferOwnershipAsync(ClaimsPrincipal user, Guid conversationId, Guid newOwnerId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, newOwnerId, ConversationPermissions.TransferOwnership);

            var currentOwnerId = user.GetUserId();

            var oldOwner = await unitOfWork.ConversationMember.GetActiveAsync(conversationId, user.GetUserId())
                 ?? throw new NotFoundException("Owner not found.");
            var newOwner = await unitOfWork.ConversationMember.GetActiveAsync(conversationId, newOwnerId)
                ?? throw new NotFoundException("New owner not found.");

            if ( oldOwner.Role != ConversationRole.Owner)
                throw new ForbiddenException("Only the owner can transfer ownership.");

            oldOwner.Role = ConversationRole.Admin;
            newOwner.Role = ConversationRole.Owner;


            await unitOfWork.SaveChangesAsync();
        }

        public async Task<ConversationDetailsDto> UpdateAsync(ClaimsPrincipal user,Guid conversationId, UpdateGroupDto dto)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.ManageGroup);

            var group =
                await unitOfWork.Groups
                    .GetByIdAsync(conversationId)
                ?? throw new NotFoundException("Group not found.");

            if (dto.Title is not null)
                group.Title = dto.Title.Trim();

            if (dto.Description is not null)
                group.Description = dto.Description.Trim();

            await unitOfWork.SaveChangesAsync();

            var result = mapper.Map<ConversationDetailsDto>(group);

            return result;
        }
  
        public async Task BanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId, BanMemberDto dto)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, userId,ConversationPermissions.BanMember);
            var member = await unitOfWork.ConversationMember.GetAsync(conversationId, userId);
            if (member == null || member.LeftAt !=null)
            {
                throw new NotFoundException("Member not found.");
            }

            member.IsBanned = true;
            member.BannedAt = timeProvider.GetUtcNow().UtcDateTime;
            member.BannedById = user.GetUserId();
            member.BanReason = dto.Reason;
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
            member.BanReason = null;
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MuteMemberAsync(ClaimsPrincipal user,Guid conversationId,Guid userId, MuteConversationDto dto)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId,userId, ConversationPermissions.MuteMember);
            var member = await unitOfWork.ConversationMember.GetActiveAsync(conversationId, userId);
            if (member == null)
            {
                throw new NotFoundException("Member not found.");
            }
            member.IsMutedByAdmin = true;
            member.MutedUntil = dto.MuteUntil;
            member.MutedById = user.GetUserId();
           await unitOfWork.SaveChangesAsync();
        }
        public async Task UnMuteMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid userId)
        {
            await authorizationService.AuthorizeMemberAsync(user, conversationId, userId,ConversationPermissions.UnmuteMember);
            var member = await unitOfWork.ConversationMember.GetActiveAsync(conversationId, userId);
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
