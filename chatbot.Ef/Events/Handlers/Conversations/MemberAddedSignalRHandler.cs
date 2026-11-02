using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class MemberAddedSignalRHandler(IChatRealtimeService realtimeService,IUnitOfWork unitOfWork) : IEventHandler<MemberAddedEvent>
    {
        public async Task HandleAsync(MemberAddedEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.ConversationId,
                @event.UserId,
                @event.AddedByUserId,
                @event.Role,
                @event.AddedAt
            };

            // Notify current group members.
            await realtimeService.SendToConversationAsync(@event.ConversationId, "MemberAdded", payload,cancellationToken);

            // Notify the added user directly.
            await realtimeService.SendToUserAsync( @event.UserId, "AddedToGroup",payload, cancellationToken);
        }
    }
}
