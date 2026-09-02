using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface INotificationsService
    {
        Task CreateAsync(Guid userId, string title, string body);
        Task<List<Notification>> GetNotificationsAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
    }
}
