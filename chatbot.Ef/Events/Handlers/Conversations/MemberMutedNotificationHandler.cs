using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class MemberMutedNotificationHandler(INotificationService notificationService) : IEventHandler<MemberMutedEvent>
    {
        public Task HandleAsync( MemberMutedEvent @event,CancellationToken cancellationToken = default)
        {
            return notificationService.SendAsync(@event.UserId, NotificationType.MemberMuted, "You were muted",
                @event.MutedUntil.HasValue
                    ? $"You have been muted until {@event.MutedUntil.Value:u}."
                    : "You have been muted by a group admin.",
                @event.ConversationId, cancellationToken: cancellationToken);
        }
    }
}
