using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);

        Task<ApplicationUser?> GetByNameAsync(string username);

        Task<ApplicationUser?> GetByTokenAsync(string token);

        Task<IdentityResult> CreateUserAsync(
            ApplicationUser user,
            string password);

        Task<bool> CheckPasswordAsync(
            ApplicationUser user,
            string password);

        // Device Session
        Task<DeviceSession> CreateDeviceSessionAsync(
            DeviceSession session);

        Task<DeviceSession?> GetDeviceSessionAsync(
            Guid sessionId,
            Guid userId);

        Task RevokeDeviceSessionAsync(
            DeviceSession session);

        // Refresh Token
        Task SaveRefreshTokenAsync(
            RefreshToken refreshToken);

        Task<RefreshToken?> GetRefreshTokenAsync(
            string tokenHash);

        Task RevokeRefreshTokenAsync(
            RefreshToken refreshToken);
    }
}
