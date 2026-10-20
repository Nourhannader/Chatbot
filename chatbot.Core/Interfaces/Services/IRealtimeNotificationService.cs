using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Notifications;

namespace chatbot.Core.Interfaces.Services
{
    public interface IRealtimeNotificationService
    {
        Task SendAsync(Guid userId, NotificationDto notification);
        Task SendUnreadCountAsync(Guid userId,int unreadCount);
    }
}
