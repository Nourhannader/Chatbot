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
    public sealed class MemberUnbannedNotificationHandler( INotificationService notificationService): IEventHandler<MemberUnbannedEvent>
    {
        public Task HandleAsync(MemberUnbannedEvent @event, CancellationToken cancellationToken = default)
        {
            return notificationService.SendAsync( @event.UserId, NotificationType.MemberUnbanned,
                "Ban removed","Your group ban has been removed. You can be added to the group again.",
                @event.ConversationId,cancellationToken: cancellationToken);
        }
    }
}
