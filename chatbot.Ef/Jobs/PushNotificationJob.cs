using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class PushNotificationJob
    {
        private readonly INotificationService notificationService;
        private readonly ILogger<PushNotificationJob> logger;
        public PushNotificationJob(INotificationService notificationService,ILogger<PushNotificationJob> logger)
        {
            this.notificationService = notificationService;
            this.logger = this.logger;
        }
        [AutomaticRetry(Attempts = 3)]
        public async Task SendAsync(Guid userId,NotificationType type,string title,string body,NotificationOptions? options = null)
        {
            logger.LogInformation("Push notification job started for user {UserId}", userId);
            await notificationService.SendAsync(userId, type, title, body, options);
            logger.LogInformation("Push notification Sent to user {UserId}", userId);
        }
    }
}
