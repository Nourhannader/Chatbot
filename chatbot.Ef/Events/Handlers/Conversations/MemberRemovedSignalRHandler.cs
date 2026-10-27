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
    public sealed class MemberRemovedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberRemovedEvent>
    {
        public async Task HandleAsync(MemberRemovedEvent @event,CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToConversationAsync( @event.ConversationId, "MemberRemoved",
                new
                {
                    @event.ConversationId,
                    @event.UserId,
                    @event.RemovedByUserId,
                    @event.RemovalType,
                    @event.RemovedAt
                },
                cancellationToken);

            await realtimeService.SendToUserAsync( @event.UserId, "RemovedFromConversation",
                new
                {
                    @event.ConversationId,
                    @event.RemovalType,
                    @event.RemovedAt
                },
                cancellationToken);
        }
    }
}
