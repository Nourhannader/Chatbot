using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class NotificationsService(IUnitOfWork unitOfWork) : INotificationsService
    {
        public async Task CreateAsync(string userId, string title, string body)
        {
            await unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = userId,
                Title = title,
                Body = body,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsAsync(string userId)
        {
            return await unitOfWork.Notifications.GetUserNotificationsAsync(userId);
        }

        public async Task MarkAsReadAsync(string notificationId)
        {
            var notification = await unitOfWork.Notifications.GetByIdAsync(notificationId);
            if(notification == null)
            {
                return;
            }
            notification.IsRead = true;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
