using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class MemberRemovedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberRemovedEvent>
    {
        public async Task HandleAsync(MemberRemovedEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.ConversationId,
                @event.UserId,
                @event.RemovedByUserId,
                @event.RemovalType,
                @event.RemovedAt
            };

            // Notify remaining group members.
            await realtimeService.SendToConversationAsync(@event.ConversationId, "MemberRemoved",payload, cancellationToken);

            // Notify the affected user directly.
            var userMethod = @event.RemovalType switch
            {
                MemberRemovalType.Left => "LeftGroup",
                MemberRemovalType.Removed => "RemovedFromGroup",
                MemberRemovalType.Banned => "BannedFromGroup",
                _ => "MemberRemoved"
            };

            await realtimeService.SendToUserAsync( @event.UserId, userMethod,payload,cancellationToken);
        }
    }
}
