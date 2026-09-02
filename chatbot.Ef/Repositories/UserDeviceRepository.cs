using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class UserDeviceRepository(ApplicationDbContext context) : IUserDeviceRepository
    {
        public async Task AddAsync(UserDevice entity)
        {
           await context.UserDevices.AddAsync(entity);

        }

        public async Task<UserDevice?> GetByIdAsync(Guid id)
        {
            return await context.UserDevices
                .FirstOrDefaultAsync(ud => ud.Id == id);
        }

        public async Task<UserDevice?> GetByUserAsync(Guid userId, string deviceToken)
        {
            return await context.UserDevices
                .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DeviceToken == deviceToken);
        }

        public async Task<List<UserDevice>> GetUserDevicesAsync(Guid userId)
        {
            return await context.UserDevices
                .Where(ud => ud.UserId == userId)
                .ToListAsync();
        }

        public void Update(UserDevice entity)
        {
            context.UserDevices.Update(entity);
        }
    }
}
