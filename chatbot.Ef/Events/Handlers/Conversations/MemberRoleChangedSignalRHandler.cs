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
    public sealed class MemberRoleChangedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberRoleChangedEvent>
    {
        public async Task HandleAsync(MemberRoleChangedEvent @event, CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToConversationAsync(@event.ConversationId, "MemberRoleChanged",
                new
                {
                    @event.ConversationId,
                    @event.UserId,
                    @event.OldRole,
                    @event.NewRole,
                    @event.ChangedByUserId,
                    @event.ChangedAt
                },
                cancellationToken);
        }
    }
}
