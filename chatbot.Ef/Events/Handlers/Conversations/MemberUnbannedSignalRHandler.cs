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
    public sealed class MemberUnbannedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberUnbannedEvent>
    {
        public async Task HandleAsync( MemberUnbannedEvent @event, CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.ConversationId,
                @event.UserId,
                @event.UnbannedByUserId,
                @event.UnbannedAt
            };

            await realtimeService.SendToUserAsync( @event.UserId, "UnbannedFromGroup", payload, cancellationToken);
        }
    }
}
