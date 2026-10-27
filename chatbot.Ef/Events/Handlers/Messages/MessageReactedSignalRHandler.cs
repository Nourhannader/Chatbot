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
    public sealed class MessageReactedSignalRHandler( IChatRealtimeService realtimeService): IEventHandler<MessageReactedEvent>
    {
        public async Task HandleAsync( MessageReactedEvent @event, CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToConversationAsync( @event.ConversationId, "MessageReactionChanged",
                new
                {
                    @event.MessageId,
                    @event.ConversationId,
                    @event.UserId,
                    @event.ReactionType,
                    @event.IsRemoved,
                    @event.ReactedAt
                },
                cancellationToken);
        }
    }
}
