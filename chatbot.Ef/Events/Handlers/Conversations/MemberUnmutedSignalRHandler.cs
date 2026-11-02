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
    public sealed class MemberUnmutedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberUnmutedEvent>
    {
        public async Task HandleAsync( MemberUnmutedEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.ConversationId,
                @event.UserId,
                @event.UnmutedByUserId,
                @event.UnmutedAt
            };

            await realtimeService.SendToConversationAsync(@event.ConversationId,"MemberUnmuted",payload, cancellationToken);

            await realtimeService.SendToUserAsync(@event.UserId, "YouWereUnmuted",payload, cancellationToken);
        }
    }                
}
