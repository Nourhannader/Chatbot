using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface INotificationRepository:IBaseRepository<Notification,string>
    {
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
    }
}
