using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using FirebaseAdmin.Messaging;

namespace chatbot.Ef.Services
{
    public class FirebaseNotificationService(IUnitOfWork unitOfWork) : IPushNotificationService
    {
        public async Task SendAsync(string pushToken, string title,string body,
        Dictionary<string, string>? data = null)
        {
            var message = new Message
            {
                Token = pushToken,

                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },

                Data = data
            };

            await FirebaseMessaging
                .DefaultInstance
                .SendAsync(message);
        }
    }
}
