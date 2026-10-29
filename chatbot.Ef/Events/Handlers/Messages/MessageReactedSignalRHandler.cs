using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Reactions;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageReactedSignalRHandler( IChatRealtimeService realtimeService): IEventHandler<MessageReactedEvent>
    {
        public async Task HandleAsync( MessageReactedEvent @event, CancellationToken cancellationToken = default)
        {
            var dto = new ReactionResultDto
            {
                MessageId = @event.MessageId,
                ConversationId = @event.ConversationId,
                UserId = @event.UserId,
                ReactionType = @event.ReactionType,
                IsRemoved = @event.IsRemoved,
                ReactedAt = @event.ReactedAt
            };

            await realtimeService.MessageReactionChangedAsync(@event.ConversationId, dto);
        }
    }
}
