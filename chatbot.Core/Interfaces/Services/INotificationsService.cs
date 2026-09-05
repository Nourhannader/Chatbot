using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task CreateAsync(
            Guid userId,
            string title,
            string body,
            NotificationType type);


        Task MarkAsReadAsync(
            Guid notificationId,
            Guid userId);


        Task<IEnumerable<Notification>>
            GetUserNotificationsAsync(
                Guid userId);


        Task<IEnumerable<Notification>>
            GetUnreadNotificationsAsync(
                Guid userId);
    }
}
