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
    public sealed class MemberUnmutedNotificationHandler(INotificationService notificationService): IEventHandler<MemberUnmutedEvent>
    {
        public Task HandleAsync(MemberUnmutedEvent @event, CancellationToken cancellationToken = default)
        {
            return notificationService.SendAsync(
                @event.UserId,
                NotificationType.MemberUnmuted,
                "Mute removed",
                "Your group mute has been removed.",
                @event.ConversationId,
                cancellationToken: cancellationToken);
        }
    }
}
