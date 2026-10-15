using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public class ConversationService(IUnitOfWork unitOfWork, IMapper mapper,IStorageService storageService
        , IConversationAuthorizationService authorizationService,TimeProvider timeProvider) : IConversationService
    {
        public async Task<ConversationDetailsDto> CreatePrivateAsync(ClaimsPrincipal user, CreatePrivateConversationDto dto)
        {
            var currentUserId = user.GetUserId();
            Guid.TryParse(dto.UserId.ToString(), out var TargetUserId);

            // Verify dto.UserId exists here.
            var targetUser = await unitOfWork.Auth
                    .GetByIdAsync(TargetUserId)
                ?? throw new NotFoundException("User not found.");

            var privateKey = CreatePrivateKey(currentUserId, TargetUserId);

            var existing = await unitOfWork.Conversations.GetPrivateByKeyAsync(privateKey);

            if (existing is not null)
            {
                return mapper.Map<ConversationDetailsDto>(existing);
            }


            var now = DateTime.UtcNow;
            var conversation =
                new Conversation
                {
                    Id = Guid.NewGuid(),
                    Type = ConversationType.Private,
                    CreatedById = currentUserId,
                    CreatedAt = now,
                    PrivateConversationKey = privateKey
                };

            await unitOfWork.Conversations.AddAsync(conversation);

            await unitOfWork.ConversationMember
                .AddRangeAsync(
                [
                    new ConversationMember
                {
                    Id = Guid.NewGuid(),
                    ConversationId =conversation.Id,
                    UserId =currentUserId,
                    Role =ConversationRole.Member,
                    JoinedAt = now
                },

                new ConversationMember
                {
                    Id = Guid.NewGuid(),
                    ConversationId =conversation.Id,
                    UserId =TargetUserId,
                    Role =ConversationRole.Member,
                    JoinedAt = now
                }
                ]);

            await unitOfWork.SaveChangesAsync();

            var result = mapper.Map<ConversationDetailsDto>(conversation);

            return result;
        }

        public async Task<ConversationDetailsDto> GetDetailsAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.View);

            var conversation = await unitOfWork.Conversations
                    .GetDetailsAsync(conversationId)
                ?? throw new NotFoundException("Conversation not found.");

            var dto = mapper.Map<ConversationDetailsDto>(conversation);

            var currentMember = conversation.Members
                       .FirstOrDefault(x => x.UserId ==
                            user.GetUserId() &&
                        x.LeftAt == null && !x.IsBanned);

            dto.CurrentUserRole = currentMember?.Role;

            return dto;
        }

        public async Task<List<ConversationDto>> GetMineAsync(ClaimsPrincipal user)
        {
            var conversations = await unitOfWork.Conversations
                    .GetUserConversationsAsync(user.GetUserId());

            return mapper.Map<List<ConversationDto>>(conversations);
        }
      
        public async Task<List<ConversationDto>> GetMyConversationsAsync(ClaimsPrincipal user)
        {
            var userId = user.GetUserId();

            var conversations = await unitOfWork.Conversations
                    .GetUserConversationsAsync(userId);

            return mapper.Map<List<ConversationDto>>(conversations);
        }

        private static string CreatePrivateKey(Guid first,Guid second)
        {
            var firstString =first.ToString("N");

            var secondString =second.ToString("N");

            return string.CompareOrdinal(
                       firstString,
                       secondString) < 0
                ? $"{firstString}:{secondString}"
                : $"{secondString}:{firstString}";
        }

        public async Task<ConversationImageDto> AddOrUpdateImageAsync(ClaimsPrincipal user, Guid conversationId, UpdateConversationImageDto dto)
        {
            await authorizationService.AuthorizeImageManagementAsync(user, conversationId);

            var conversation = await unitOfWork.Conversations
            .GetWithImageAsync(conversationId)
                ?? throw new NotFoundException("Conversation not found.");

            var oldImage = conversation.Image;

            UploadResultDto? upload = null;

            StoredFile? newImage = null;

            var committed = false;

            try
            {
               
                    upload = await storageService.UploadConversationImageAsync(dto.Image,conversationId,user.GetUserId());
                    newImage = new StoredFile
                    {
                        Id = Guid.NewGuid(),
                        StorageKey = upload.StorageKey,
                        OriginalFileName = dto.Image.FileName,
                        StoredFileName = upload.FileName,
                        ContentType = upload.ContentType,
                        FileSize = upload.Size,
                        CreatedAt = timeProvider.GetUtcNow().UtcDateTime
                    };
                    await unitOfWork.StoredFiles.AddAsync(newImage);
                    conversation.ImageId = newImage.Id;
                    conversation.Image = newImage;
                await unitOfWork.SaveChangesAsync();
                committed = true;
            }
            catch
            {
                if (!committed && upload is not null)
                {
                    await TryDeletePhysicalFileAsync(upload.StorageKey);
                }

                throw;
            }

            // DB points to new image successfully.

            if (oldImage is not null)
            {
                await DeleteOldImageSafelyAsync(oldImage);
            }

            return mapper.Map<ConversationImageDto>(newImage!);
        }

        public async Task RemoveImageAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeImageManagementAsync(user,conversationId);

            var conversation =
                await unitOfWork.Conversations
                    .GetWithImageAsync(conversationId)
                ?? throw new NotFoundException("Conversation not found.");

            if (conversation.Image is null)
                return;

            var oldImage =
                conversation.Image;

            conversation.ImageId = null;
            conversation.Image = null;

            await unitOfWork.SaveChangesAsync();

            //BackgroundJobClient.Enqueue<
            //    DeleteStoredFileJob>(
            //        job => job.ExecuteAsync(
            //            oldImage.Id));
        }


        private async Task TryDeletePhysicalFileAsync(string storageKey)
        {
            try
            {
                await storageService.DeleteAsync(storageKey);
            }
            catch
            {
                // Log the error.
                // Never hide the original exception.
                //
                // CleanupOrphanFilesJob can remove
                // orphaned physical files later.
            }
        }
    }
}
