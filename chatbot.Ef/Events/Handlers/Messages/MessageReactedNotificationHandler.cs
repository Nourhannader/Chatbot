using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageReactedNotificationHandler( INotificationService notificationService): IEventHandler<MessageReactedEvent>
    {
        public async Task HandleAsync(MessageReactedEvent @event,CancellationToken cancellationToken = default)
        {
            if (@event.IsRemoved)
                return;

            await notificationService.CreateReactionNotificationAsync(
                @event.MessageId,
                @event.UserId,
                @event.ReactionType,
                cancellationToken);
        }
    }
}
