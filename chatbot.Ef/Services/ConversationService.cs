using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using chatbot.Ef.ValidatorService;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Services
{
    public class ConversationService(IUnitOfWork unitOfWork, IMapper mapper,IStorageService storageService,IFileValidationService fileValidationService
        , IConversationAuthorizationService authorizationService,TimeProvider timeProvider,ILogger<ConversationService> logger) : IConversationService
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

            var conversation = await unitOfWork.Conversations .GetWithImageAsync(conversationId)
                ?? throw new NotFoundException("Conversation not found.");

            // Check permission before uploading

            await authorizationService.AuthorizeImageManagementAsync(user, conversationId);

            var currentUserId = user.GetUserId();
            var validated = await fileValidationService.ValidateImageAsync(dto.Image);

            var oldImage = conversation.Image;

            StoredFile? newImage = null;

            UploadResultDto? upload = null;

            var committed = false;

            try
            {
                newImage = await storageService.UploadAsync(dto.Image, validated, FileCategory.ConversationImage, conversationId, user.GetUserId());
                upload = new UploadResultDto
                {
                    Success = true,
                    FileId = newImage.Id,
                    StorageKey = newImage.StorageKey,
                    FileUrl = storageService.GetFileUrl(newImage.StorageKey, newImage.Provider),
                    ContentType = newImage.ContentType,
                    Size = newImage.FileSize,
                    FileName = newImage.StoredFileName
                };

                await unitOfWork.StoredFiles.AddAsync(newImage);
                conversation.ImageId = newImage.Id;
                conversation.Image = newImage;
                await unitOfWork.SaveChangesAsync();
                committed = true;
            }
            catch
            {
                if (!committed && newImage is not null)
                {
                    await TryDeleteFileAsync(newImage);
                }

                throw;
            }

            // DB points to new image successfully.

            if (oldImage is not null)
            {
                await TryDeleteFileAsync(oldImage);
            }

            return mapper.Map<ConversationImageDto>(newImage!);
        }

        public async Task RemoveImageAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeImageManagementAsync(user,conversationId);

            var conversation =await unitOfWork.Conversations.GetWithImageAsync(conversationId)
                ?? throw new NotFoundException("Conversation not found.");

            if (conversation.Image is null)
                return;

            var oldImage =conversation.Image;

            conversation.ImageId = null;
            conversation.Image = null;

            await unitOfWork.SaveChangesAsync();

            try
            {
                await storageService.DeleteAsync(oldImage,CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Conversation image relation was removed, but old file {FileId} " + "could not be deleted from storage.",oldImage.Id);

                // Do NOT throw.
                //
                // conversation image has already been removed successfully.
                // Old orphan file can be cleaned later using Hangfire.
            }

            //BackgroundJobClient.Enqueue<
            //    DeleteStoredFileJob>(
            //        job => job.ExecuteAsync(
            //            oldImage.Id));
        }

        public async Task<StoredFile>UpdateConversationImageAsync(Guid conversationId,ClaimsPrincipal currentUser,IFormFile image, CancellationToken cancellationToken = default)
        {
            var conversation = await unitOfWork.Conversations.GetByIdAsync(conversationId)
                     ?? throw new NotFoundException("Conversation not found.");

            // Check permission before uploading

           await authorizationService.AuthorizeImageManagementAsync(currentUser, conversationId);
           
            var currentUserId=currentUser.GetUserId();
            var validated =await fileValidationService.ValidateImageAsync( image,cancellationToken);

            var oldImage =conversation.Image;

            StoredFile? newImage = null;

            try
            {
                newImage =await storageService.UploadAsync(
                        image,
                        validated,
                        FileCategory.ConversationImage,

                        // Important:
                        // owner is Conversation
                        conversationId,

                        // uploader is current user
                        currentUserId,

                        cancellationToken);

                conversation.ImageId =newImage.Id;
                conversation.Image =newImage;

                await unitOfWork.SaveChangesAsync();
            }
            catch
            {
                if (newImage is not null)
                {
                    await TryDeleteFileAsync( newImage);
                }

                throw;
            }

            if (oldImage is not null)
            {
                await TryDeleteFileAsync(oldImage);
            }

            return newImage;
        }

        private async Task TryDeleteFileAsync(StoredFile file)
        {
            try
            {
                await storageService.DeleteAsync(file, CancellationToken.None);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete old file {FileId} " + "with storage key {StorageKey}.", file.Id, file.StorageKey);
            }
        }
    }
}
