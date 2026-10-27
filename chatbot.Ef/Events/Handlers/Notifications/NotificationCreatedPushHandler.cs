using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Notifications;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Events.Handlers.Notifications
{
    public sealed class NotificationCreatedPushHandler(
    INotificationPreferenceService preferencesService,IUnitOfWork unitOfWork,IPushNotificationFactory pushFactory): IEventHandler<NotificationCreatedEvent>
    {
        public async Task HandleAsync(
            NotificationCreatedEvent @event,
            CancellationToken cancellationToken = default)
        {
            var preferences = await preferencesService.GetAsync( @event.UserId);

            if (!preferences.PushNotifications)
                return;

            var devices =await unitOfWork.UserDevices.GetActiveDevicesAsync( @event.UserId);

            foreach (var device in devices)
            {
                if (string.IsNullOrWhiteSpace(device.PushToken))
                {
                    continue;
                }

                var pushService =pushFactory.Get( device.Provider);

                await pushService.SendAsync( device.PushToken,@event.Title, @event.Body,
                    new Dictionary<string, string>
                    {
                        ["notificationId"] = @event.NotificationId.ToString(),

                        ["type"] = @event.Type.ToString(),

                        ["conversationId"] = @event.ConversationId?.ToString() ?? string.Empty,

                        ["messageId"] =@event.MessageId?.ToString() ?? string.Empty
                    });
            }
        }
    }
}
