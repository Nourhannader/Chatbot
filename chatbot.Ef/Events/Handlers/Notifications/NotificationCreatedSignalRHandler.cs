using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Notifications;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Notifications
{
    public sealed class NotificationCreatedSignalRHandler( IChatRealtimeService realtimeService) : IEventHandler<NotificationCreatedEvent>
    {
        public async Task HandleAsync(NotificationCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToUserAsync(@event.UserId,"NotificationCreated",
                new
                {
                    @event.NotificationId,
                    @event.Type,
                    @event.Title,
                    @event.Body,
                    @event.ConversationId,
                    @event.MessageId,
                    @event.CreatedAt
                },
                cancellationToken);
        }
    }
}
