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
    public class NotificationRepository(ApplicationDbContext context) : INotificationRepository
    {
        public async Task AddAsync(Notification entity)
        {
            await context.Notifications.AddAsync(entity);
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<Notification>> GetUnreadUserNotificationsAsync(Guid userId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            return await context.Notifications
                .Where(n=> n.UserId == userId)
                .OrderByDescending(n=> n.CreatedAt)
                .ToListAsync();
        }

        public void Update(Notification entity)
        {
            context.Notifications.Update(entity);
        }
    }
}
