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
    public sealed class NotificationPreferenceRepository(ApplicationDbContext context) : INotificationPreferenceRepository
    {
        public async Task AddAsync(NotificationPreferences entity)
        {
            await context.NotificationPreferences
                .AddAsync(entity);
        }


        public async Task<NotificationPreferences?>  GetByUserIdAsync(Guid userId)
        {
            return await context.NotificationPreferences
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId);
        }
    }
}
