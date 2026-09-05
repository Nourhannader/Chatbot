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
        public async Task SendAsync(Guid userId, string title, string body, CancellationToken cancellationToken = default)
        {
            var devices = await unitOfWork.UserDevices.GetActiveDevicesAsync(userId);
            var tokens=devices.Select(x => x.DeviceToken).ToList();

            if (tokens.Any())
            {
                return;
            }
            var message =
            new MulticastMessage
            {
                Tokens = tokens,

                Notification =
                    new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = body
                    }
            };


            await FirebaseMessaging
                .DefaultInstance
                .SendEachForMulticastAsync(
                    message,
                    cancellationToken);
        }
    }
}
