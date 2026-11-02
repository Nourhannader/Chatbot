using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class ConversationUpdatedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<ConversationUpdatedEvent>
    {
        public Task HandleAsync(ConversationUpdatedEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.ConversationId,
                @event.UpdatedByUserId,
                @event.Title,
                @event.Description,
                @event.UpdatedAt
            };

            return realtimeService.SendToConversationAsync( @event.ConversationId, "ConversationUpdated", payload, cancellationToken);
        }
    }
}
