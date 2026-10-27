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
    public sealed class MessageReplyNotificationHandler(INotificationService notificationService): IEventHandler<MessageReplyCreatedEvent>
    {
        public async Task HandleAsync( MessageReplyCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.SenderId == @event.OriginalMessageSenderId)
            {
                return;
            }

            await notificationService.CreateReplyNotificationAsync(
                @event.MessageId,
                @event.ReplyToMessageId,
                @event.ConversationId,
                @event.SenderId,
                @event.OriginalMessageSenderId,
                cancellationToken);
        }
    }
}
