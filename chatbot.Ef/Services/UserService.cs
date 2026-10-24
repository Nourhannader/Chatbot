using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using chatbot.Ef.ValidatorService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Services
{
    public class UserService(IUnitOfWork unitOfWork,IStorageService storageService,IFileValidationService fileValidation
        ,ILogger<UserService> logger) : IUserService
    {
        //mapping
        private static UserProfileDto MapToDto(ApplicationUser user,string fileUrl)
        {
            return new UserProfileDto
            {
                Id = user.Id,

                FirstName =
                    user.FirstName ?? string.Empty,

                LastName =
                    user.LastName ?? string.Empty,

                UserName =
                    user.UserName ?? string.Empty,

                Email =
                    user.Email,

                Phone =
                    user.PhoneNumber,

                Bio =
                    user.Bio,

                ProfileImageUrl =
                    fileUrl,

                IsOnline =
                    user.IsOnline,

                LastSeenAt =
                    user.LastSeenAt
            };
        }
        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await unitOfWork.Auth.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            string fileUrl = string.Empty;

            if (user.ProfileImageId.HasValue && user.ProfileImage != null)
            {
                fileUrl = storageService.GetFileUrl(user.ProfileImage.StorageKey, user.ProfileImage.Provider) ?? string.Empty;
            }

            return MapToDto(user, fileUrl);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateUserDto dto)
        {
            //getuser
            var user = await unitOfWork.Auth.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }
            //checkusername
            if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.UserName)
            {
                var exists = await unitOfWork.Auth.UsernameExistsAsync(dto.UserName, userId);

                if (exists)
                {
                    throw new ConflictException("Username is already taken.");
                }

                user.UserName = dto.UserName.Trim();
            }
            //update info
            if (!string.IsNullOrWhiteSpace(dto.FirstName))
            {
                user.FirstName = dto.FirstName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.LastName))
            {
                user.LastName = dto.LastName.Trim();
            }

            if (dto.Phone != null)
            {
                user.PhoneNumber = dto.Phone.Trim();
            }

            if (dto.Bio != null)
            {
                user.Bio = dto.Bio.Trim();
            }

            string fileUrl = string.Empty;
            //update image
            if (dto.ImageFile != null)
            {
                var newImage = await UpdateProfileImageAsync(userId, dto.ImageFile);
                fileUrl = newImage.FileUrl; 
            }

            await unitOfWork.Auth.updateState(user);

            await unitOfWork.SaveChangesAsync();
            return MapToDto(user, fileUrl);
        }

        public async Task<StoredFile> UpdateProfileImageAsync(Guid userId, IFormFile image, CancellationToken cancellationToken = default)
        {
            var user = await unitOfWork.Auth.GetByIdAsync(userId)
                    ?? throw new NotFoundException("User not found.");

            // 1. Validate
            var validated = await fileValidation.ValidateImageAsync(image, cancellationToken);

            var oldImage = user.ProfileImage;

            StoredFile? newImage = null;

            try
            {
                // 2. Upload NEW image
                newImage =
                    await storageService.UploadAsync(
                        image, validated, FileCategory.UserProfileImage,
                        // ownerId
                        userId,
                        // uploadedById
                        userId,
                        cancellationToken);

                // 3. Change relation
                user.ProfileImageId = newImage.Id;

                user.ProfileImage = newImage;

                // 4. Save relation
                await unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // New image uploaded but updating
                // the user may have failed.
                if (newImage is not null)
                {
                    await TryDeleteFileAsync(newImage);
                }

                throw;
            }

            // 5. Delete old image AFTER
            // successful replacement.
            if (oldImage is not null)
            {
                await TryDeleteFileAsync(oldImage);
            }

            return newImage;

        }

        public async Task DeleteProfileImageAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await unitOfWork.Auth.GetByIdWithProfileImageAsync(userId );

            if (user is null)
            {
                throw new NotFoundException(
                    "User not found.");
            }

            var oldImage = user.ProfileImage;

            if (oldImage is null)
            {
                return;
            }

            // =====================================
            // 1. Remove relationship first
            // =====================================

            user.ProfileImageId = null;
            user.ProfileImage = null;

            await unitOfWork.SaveChangesAsync();


            // =====================================
            // 2. Delete old StoredFile
            // =====================================

            try
            {
                await storageService.DeleteAsync(oldImage,CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError( ex,"Profile image relation was removed, but old file {FileId} " +"could not be deleted from storage.",oldImage.Id);

                // Do NOT throw.
                //
                // User profile image has already been removed successfully.
                // Old orphan file can be cleaned later using Hangfire.
            }
        }

        private async Task TryDeleteFileAsync(StoredFile file)
        {
            try
            {
                await storageService.DeleteAsync(file,CancellationToken.None);

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Failed to delete old file {FileId} " +"with storage key {StorageKey}.", file.Id,file.StorageKey);
            }
        }



    }
}
