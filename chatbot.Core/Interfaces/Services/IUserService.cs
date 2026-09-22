using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Auth;

namespace chatbot.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);

        Task<UserProfileDto> UpdateProfileAsync(Guid userId,UpdateUserDto dto);
    }
}
