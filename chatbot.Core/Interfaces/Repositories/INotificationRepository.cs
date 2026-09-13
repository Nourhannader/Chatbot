using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface INotificationRepository:IBaseRepository<Notification,Guid>
    {
        Task<Notification?> GetByUserIdAsync(Guid notificationId, Guid userId);

        Task<List<NotificationDto>> GetAsync(Guid userId,int pageNumber, int pageSize);

        Task<int> GetUnreadCountAsync(Guid userId);

        Task MarkAsReadAsync(Guid notificationId,Guid userId);

        Task MarkAllAsReadAsync( Guid userId);
    }
}
