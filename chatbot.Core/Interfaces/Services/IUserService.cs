using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);

        Task<UserProfileDto> UpdateProfileAsync(Guid userId,UpdateUserDto dto);

        Task<StoredFile> UpdateProfileImageAsync(Guid userId, IFormFile image, CancellationToken cancellationToken = default);
        Task DeleteProfileImageAsync( Guid userId,CancellationToken cancellationToken = default);
    }
}
