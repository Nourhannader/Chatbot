using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class UserService(IUnitOfWork unitOfWork,IStorageService storageService) : IUserService
    {
        //mapping
        private static UserProfileDto MapToDto(
       ApplicationUser user,string fileUrl)
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

            if (user.ProfileImageId.HasValue)
            {
                fileUrl = await storageService.GetFileUrlAsync(user.ProfileImageId.Value) ?? string.Empty;
            }

            return MapToDto(user, fileUrl);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateUserDto dto)
        {
            //getuser
            var user =await unitOfWork.Auth.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }
            //checkusername
            if (!string.IsNullOrWhiteSpace(dto.UserName) &&dto.UserName != user.UserName)
            {
                var exists =await unitOfWork.Auth.UsernameExistsAsync(dto.UserName,userId);

                if (exists)
                {
                    throw new ConflictException("Username is already taken.");
                }

                user.UserName =dto.UserName.Trim();
            }
            //update info
            if (!string.IsNullOrWhiteSpace(dto.FirstName))
            {
                user.FirstName =dto.FirstName.Trim();
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
            var newImage = new UploadResultDto();
            //update image
            if (dto.ImageFile != null)
            {
                var oldImageId = user.ProfileImageId;
                newImage =
                    await storageService.ReplaceUserProfileImageAsync(dto.ImageFile,userId,oldImageId);

                user.ProfileImageId = newImage.FileId;
            }
            var fileUrl = newImage.FileUrl;
            await unitOfWork.Auth.updateState(user);

            await unitOfWork.SaveChangesAsync();
            return MapToDto(user,fileUrl);
        }
    }
}
