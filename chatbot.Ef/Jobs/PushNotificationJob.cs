using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    [Queue("notifications")]
    [AutomaticRetry(Attempts = 5)]
    public sealed class PushNotificationJob(INotificationService notificationService, ILogger<PushNotificationJob> logger) : IPushNotificationJob
    {
        public async Task SendAsync(Guid userId, NotificationType type, string title, string body, NotificationOptions? options = null)
        {
            logger.LogInformation("Sending notification {title}", title);

            await notificationService.SendAsync(userId, type, title, body, options);

            logger.LogInformation("Notification {title} sent", title);
        }
    }
}
