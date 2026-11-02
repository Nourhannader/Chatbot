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
    public sealed class ConversationReadSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<ConversationReadEvent>
    {
        public async Task HandleAsync(
            ConversationReadEvent @event,
            CancellationToken cancellationToken = default)
        {
            await realtimeService.ConversationReadAsync(
                @event.ConversationId,
                @event.UserId,
                @event.ReadAt);
        }
    }
}
