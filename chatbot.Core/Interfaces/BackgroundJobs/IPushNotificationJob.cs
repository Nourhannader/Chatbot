using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.BackgroundJobs
{
    public interface IPushNotificationJob
    {
        Task SendAsync(Guid userId, NotificationType type, string title, string body, NotificationOptions? options = null);
    }
}
