using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<ApplicationUser?> GetByNameAsync(string username)
        {
            return await userManager.FindByNameAsync(username);
        }
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
        //device session
        public async Task<DeviceSession> CreateDeviceSessionAsync(DeviceSession session)
        {
             await context.Sessions.AddAsync(session);
            return session;
        }
        public async Task<DeviceSession?> GetDeviceSessionAsync(Guid sessionId, Guid userId)
        {
            return await context.Sessions.FirstOrDefaultAsync(s => s.Id==sessionId && s.UserId == userId);
        }
        public async Task RevokeDeviceSessionAsync(DeviceSession session)
        {
            session.IsActive = false;
            session.RevokedAt = DateTime.UtcNow;
            await Task.CompletedTask;
        }

        //refresh token
        public Task<ApplicationUser?> GetByTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash)
        {
            return await context.RefreshTokens
                .Include(r => r.User)
                .Include(r=> r.DeviceSession)
                .FirstOrDefaultAsync(r => r.ReplacedByTokenHash==tokenHash);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            await Task.CompletedTask;
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
        }
    }
}
