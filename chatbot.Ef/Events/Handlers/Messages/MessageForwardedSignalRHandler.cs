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
    public sealed class MessageForwardedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MessageForwardedEvent>
    {
        public async Task HandleAsync( MessageForwardedEvent @event, CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToConversationAsync( @event.TargetConversationId,"MessageForwarded",
                new
                {
                    @event.MessageId,
                    @event.OriginalMessageId,
                    @event.TargetConversationId,
                    @event.ForwardedByUserId,
                    @event.ForwardedAt
                },
                cancellationToken);
        }
    }
}
