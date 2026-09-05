using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface INotificationRepository:IBaseRepository<Notification,Guid>
    {
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<List<Notification>> GetUnreadUserNotificationsAsync(Guid userId);
    }
}
