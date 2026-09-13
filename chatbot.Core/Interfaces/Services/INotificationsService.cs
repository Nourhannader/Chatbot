using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendAsync( Guid userId,NotificationType type,string title,
        string body,NotificationOptions? options = null);

        Task<List<NotificationDto>> GetAsync(Guid userId,int pageNumber,int pageSize);

        Task MarkAsReadAsync(Guid userId,Guid notificationId);

        Task MarkAllAsReadAsync(Guid userId);

        Task<int> GetUnreadCountAsync( Guid userId);
    }
}
