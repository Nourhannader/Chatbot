using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace chatbot.Ef.Services
{
    public class OneSignalPushNotificationService(HttpClient httpClient,IConfiguration configuration) : IPushNotificationService
    {
        public async Task SendAsync(string pushToken, string title, string body, Dictionary<string, string>? data = null)
        {
            if (string.IsNullOrWhiteSpace(pushToken))
                return;

            // OneSignal-specific HTTP implementation.
            //
            // Keep provider-specific API logic here,
            // not inside NotificationService.

            await Task.CompletedTask;
        }
    }
}
