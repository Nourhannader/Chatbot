using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageDeletedSignalRHandler( IChatRealtimeService realtimeService): IEventHandler<MessageDeletedEvent>
    {
        public async Task HandleAsync(MessageDeletedEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.MessageId,
                @event.ConversationId,
                @event.UserId,
                @event.DeleteType,
                @event.DeletedAt
            };

            if (@event.DeleteType == MessageDeleteType.ForEveryone)
            {
                await realtimeService.SendToConversationAsync( @event.ConversationId,"MessageDeleted", payload, cancellationToken);

                return;
            }

            await realtimeService.SendToUserAsync(@event.UserId,"MessageDeleted",payload, cancellationToken);
        }
    }
}
