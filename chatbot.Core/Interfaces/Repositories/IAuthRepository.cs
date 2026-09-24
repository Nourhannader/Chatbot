using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        //User
        Task<ApplicationUser?> GetByEmailAsync(string email);

        Task<ApplicationUser?> GetByNameAsync(string username);
        Task<ApplicationUser?> GetByIdAsync(Guid userId);

        Task<IdentityResult> CreateUserAsync( ApplicationUser user,string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, SystemRole role);
        Task<IdentityResult> updateState(ApplicationUser user);
        Task<bool> UsernameExistsAsync(string username,Guid currentUserId);

        Task<bool> CheckPasswordAsync( ApplicationUser user,string password);

        // Device Session

        Task<DeviceSession?> GetDeviceSessionAsync(Guid sessionId);

        Task<DeviceSession?> GetDeviceSessionWithTokensAsync( Guid sessionId);

        Task<DeviceSession?> GetActiveDeviceSessionAsync(Guid userId,string deviceId);

        Task<List<DeviceSession>>GetActiveDeviceSessionsAsync(Guid userId);


        // Refresh Token
        Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash);

        Task<List<RefreshToken>>GetActiveRefreshTokensBySessionIdAsync(Guid deviceSessionId);
        //insert
        Task AddDeviceSessionAsync(DeviceSession session);

        Task AddRefreshTokenAsync(RefreshToken refreshToken);
    }
}
