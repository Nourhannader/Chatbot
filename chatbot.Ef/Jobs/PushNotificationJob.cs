using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Hangfire;

namespace chatbot.Ef.Jobs
{
    public class PushNotificationJob
    {
        private readonly INotificationService notificationService;
        public PushNotificationJob(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
        [AutomaticRetry(Attempts = 3)]
        public async Task SendAsync(Guid userId,NotificationType type,string title,string body,NotificationOptions? options = null)
        {
            await notificationService.SendAsync(userId, type, title, body, options);
        }
    }
}
