using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class AuthRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : IAuthRepository
    {
        
        //user
        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }
        public async Task<ApplicationUser?> GetByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<ApplicationUser?> GetByNameAsync(string username)
        {
            return await userManager.FindByNameAsync(username);
        }
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }
        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, SystemRole role)
        {
            return await userManager.AddToRoleAsync(user, role.ToString());
        }
        public async Task<IdentityResult> updateState(ApplicationUser user)
        {
           return await userManager.UpdateAsync(user);
        }
        public async Task<bool> UsernameExistsAsync(string username,Guid currentUserId)
        {
            return await context.Users
                .AnyAsync(x =>
                    x.UserName == username &&
                    x.Id != currentUserId);
        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
        //device session
        public async Task<DeviceSession?> GetDeviceSessionAsync(Guid sessionId)
        {
            return await context.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
        }

        public async Task<DeviceSession?> GetDeviceSessionWithTokensAsync(Guid sessionId)
        {
            return await context.Sessions
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == sessionId);
        }

        public async Task<DeviceSession?> GetActiveDeviceSessionAsync(Guid userId, string deviceId)
        {
            return await context.Sessions
                .FirstOrDefaultAsync(x => x.IsActive && x.UserId == userId && x.DeviceId == deviceId);
        }

        public async Task<List<DeviceSession>> GetActiveDeviceSessionsAsync(Guid userId)
        {
            return await context.Sessions
                .Where(x => x.UserId == userId && x.IsActive)
                .Include(x => x.RefreshTokens)
                .ToListAsync();
        }

        //refresh token
        public async Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash)
        {
            return await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

        public async Task<List<RefreshToken>> GetActiveRefreshTokensBySessionIdAsync(Guid deviceSessionId)
        {
            return await context.RefreshTokens
                .Where(x => x.DeviceSessionId == deviceSessionId &&
                x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow
                ).ToListAsync();
        }
        //insert
        public async Task AddDeviceSessionAsync(DeviceSession session)
        {
            await context.Sessions.AddAsync(session);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
        }
    }
}
