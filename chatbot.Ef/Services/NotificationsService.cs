using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class NotificationService(IUnitOfWork unitOfWork,IPushNotificationService pushNotificationService)
    : INotificationService
    {
        
        public async Task CreateAsync(
            Guid userId,
            string title,
            string body,
            NotificationType type)
        {
            var notification =
                new Notification
                {
                    Id = Guid.NewGuid(),

                    UserId = userId,

                    Title = title,

                    Body = body,

                    Type = type,

                    IsRead = false
                };


            await unitOfWork.Notifications.AddAsync(notification);


            await unitOfWork.SaveChangesAsync();


            await pushNotificationService
                .SendAsync(
                    userId,
                    title,
                    body);
        }


        public async Task MarkAsReadAsync(
            Guid notificationId,
            Guid userId)
        {
            var notification =
                await unitOfWork.Notifications
                    .GetByIdAsync(notificationId);


            if (notification == null)
                throw new Exception(
                    "Notification not found");


            if (notification.UserId != userId)
                throw new UnauthorizedAccessException();


            if (notification.IsRead)
                return;


            notification.IsRead = true;

            notification.ReadAt =
                DateTime.UtcNow;


             unitOfWork.Notifications.Update(notification); 


            await unitOfWork.SaveChangesAsync();
        }


        public async Task<IEnumerable<Notification>>
            GetUserNotificationsAsync(
                Guid userId)
        {
            return await unitOfWork.Notifications
                .GetUserNotificationsAsync(userId);
        }


        public async Task<IEnumerable<Notification>>
            GetUnreadNotificationsAsync(
                Guid userId)
        {
            return await unitOfWork.Notifications.GetUnreadUserNotificationsAsync(userId);
                
        }
    }
}
