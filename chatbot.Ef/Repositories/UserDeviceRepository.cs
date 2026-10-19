using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public sealed class UserDeviceRepository(ApplicationDbContext context) : IUserDeviceRepository
    {
        public async Task AddAsync(UserDevice entity)
        {
           await context.UserDevices.AddAsync(entity);

        }

        public async Task DeactivateAsync(Guid userId, string pushToken, PushProvider provider)
        {
            var device = await GetAsync(userId, pushToken,provider);
            if(device ==null)
                return;
            device.IsActive = false;
            device.LastUsedAt = DateTime.UtcNow;
        }

        public async Task<List<UserDevice>> GetActiveDevicesAsync(Guid userId)
        {
            return await context.UserDevices
                .AsNoTracking()
                .Where(ud => ud.UserId == userId && ud.IsActive)
                .OrderByDescending(ud => ud.LastUsedAt)
                .ToListAsync();
        }

        public async Task<UserDevice?> GetAsync(Guid userId, string pushToken, PushProvider provider)
        {
            return await context.UserDevices
                .FirstOrDefaultAsync(x => x.UserId == userId 
                && x.PushToken == pushToken
                && x.Provider ==provider);
        }

        public async Task<UserDevice?> GetByPushTokenAsync(string pushToken, PushProvider provider)
        {
            return await context.UserDevices
                .FirstOrDefaultAsync(x =>
                    x.PushToken == pushToken &&
                    x.Provider == provider);
        }

        public async Task<UserDevice?> GetByIdAsync(Guid id)
        {
            return await context.UserDevices
                .FirstOrDefaultAsync(ud => ud.Id == id);
        }

        public async Task<UserDevice?> GetByIdForUserAsync(Guid id, Guid userId)
        {
            return await context.UserDevices
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);
        }


        public async Task<IEnumerable<UserDevice>> GetUserDevicesAsync(Guid userId)
        {
           return await context.UserDevices
                .AsNoTracking()
                .Where(ud => ud.UserId == userId)
                .OrderByDescending(ud => ud.LastUsedAt)
                .ToListAsync();
        }

        public void Update(UserDevice entity)
        {
            context.UserDevices.Update(entity);
        }
    }
}
