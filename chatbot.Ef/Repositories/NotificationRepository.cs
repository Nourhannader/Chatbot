using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
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

        public async Task<List<NotificationDto>> GetAsync(Guid userId, int pageNumber, int pageSize)
        {
           return await context.Notifications.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber -1) * pageSize)
                .Take(pageSize)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Title = x.Title,
                    Body = x.Body,
                    Data = x.Data,
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt,
                    CreatedAt = x.CreatedAt
                })
            .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Notification?> GetByUserIdAsync(Guid notificationId, Guid userId)
        {
            return await context.Notifications
                .FirstOrDefaultAsync( n=> n.Id==notificationId 
                && n.UserId == userId);   
        }

        public async Task<List<Notification>> GetNotificationsReaded(DateTime? cutoff)
        {
            return await context.Notifications
                .Where(n => n.IsRead && n.CreatedAt < cutoff)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await context.Notifications
                .CountAsync(x => x.UserId == userId && !x.IsRead);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await context.Notifications
                  .Where(x => x.UserId == userId && !x.IsRead)
                  .ExecuteUpdateAsync(
                   setters => setters
                       .SetProperty(x => x.IsRead, true)
                       .SetProperty(x => x.ReadAt, DateTime.UtcNow)
                );
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await GetByUserIdAsync(notificationId, userId);
            if (notification == null)
                return;
            if (notification.IsRead)
                return;
            notification.IsRead = true;
            notification.ReadAt= DateTime.UtcNow;
        }

        public void RemoveRange(IEnumerable<Notification> notifications)
        {
            context.Notifications.RemoveRange(notifications);
            
        }

        public void Update(Notification entity)
        {
            context.Notifications.Update(entity);
        }
    }
}
