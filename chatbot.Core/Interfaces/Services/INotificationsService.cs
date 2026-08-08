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
        Task CreateAsync(string userId, string title, string body);
        Task<List<Notification>> GetNotificationsAsync(string userId);
        Task MarkAsReadAsync(string notificationId);
    }
}
