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
    public sealed class MemberMutedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberMutedEvent>
    {
        public async Task HandleAsync(MemberMutedEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.ConversationId,
                @event.UserId,
                @event.MutedByUserId,
                @event.MutedUntil,
                @event.MutedAt
            };

            await realtimeService.SendToConversationAsync(@event.ConversationId, "MemberMuted",payload, cancellationToken);

            await realtimeService.SendToUserAsync(@event.UserId,"YouWereMuted", payload,cancellationToken);
        }
    }
}
