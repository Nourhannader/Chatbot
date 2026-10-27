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
    public sealed class MessageCreatedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MessageCreatedEvent>
    {
        public async Task HandleAsync(
            MessageCreatedEvent @event,
            CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToConversationAsync( @event.ConversationId, "MessageCreated",
                new
                {
                    @event.MessageId,
                    @event.ConversationId,
                    @event.SenderId,
                    @event.Content,
                    @event.MessageType,
                    @event.SentAt
                },
                cancellationToken);
        }
    }
}
