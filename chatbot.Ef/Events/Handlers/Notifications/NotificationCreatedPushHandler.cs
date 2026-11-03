using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Enums;
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
        public async Task HandleAsync(NotificationCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            if (!@event.SendPush)
                return;


            var preferences = await preferencesService.GetAsync( @event.UserId);

            if (!preferences.PushNotifications)
                return;

            if (!IsNotificationEnabled(preferences, @event.Type))
                return;

            var devices =await unitOfWork.UserDevices.GetActiveDevicesAsync( @event.UserId);

            var data = BuildPushData(@event, preferences);


            foreach (var device in devices)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (string.IsNullOrWhiteSpace(device.PushToken))
                {
                    continue;
                }

                var pushService =pushFactory.Get( device.Provider);

                await pushService.SendAsync(device.PushToken, @event.Title, @event.Body, data);
            }
        }

        private static bool IsNotificationEnabled(NotificationPreferencesDto preferences, NotificationType type)
        {
            return type switch
            {
                NotificationType.NewMessage => preferences.NewMessages,
                NotificationType.MessageReaction => preferences.MessageReactions,
                NotificationType.MessageReply => preferences.MessageReplies,
                NotificationType.Mention => preferences.Mentions,

                _ => true
            };
        }

        private static Dictionary<string, string> BuildPushData(NotificationCreatedEvent @event, NotificationPreferencesDto preferences)
        {
            return new Dictionary<string, string>
            {
                ["notificationId"] = @event.NotificationId.ToString(),
                ["type"] = @event.Type.ToString(),
                ["conversationId"] = @event.ConversationId?.ToString() ?? string.Empty,
                ["messageId"] = @event.MessageId?.ToString() ?? string.Empty,
                ["soundEnabled"] = preferences.SoundEnabled.ToString().ToLowerInvariant()
            };
        }
    }
}
