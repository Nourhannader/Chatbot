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
    public sealed class MemberAddedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberAddedEvent>
    {
        public async Task HandleAsync(MemberAddedEvent @event,CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToConversationAsync( @event.ConversationId,"MemberAdded",
                new
                {
                    @event.ConversationId,
                    @event.UserId,
                    @event.AddedByUserId,
                    @event.Role,
                    @event.AddedAt
                },
                cancellationToken);

            await realtimeService.SendToUserAsync(@event.UserId, "AddedToConversation",
                new
                {
                    @event.ConversationId,
                    @event.Role,
                    @event.AddedAt
                },
                cancellationToken);
        }
    }
}
