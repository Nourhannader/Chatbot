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
    public sealed class MemberJoinedByInviteSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MemberJoinedByInviteEvent>
    {
        public async Task HandleAsync(MemberJoinedByInviteEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.ConversationId,
                @event.UserId,
                @event.InvitedByUserId,
                Role = ConversationRole.Member,
                @event.JoinedAt
            };

            // Notify existing conversation members.
            await realtimeService.SendToConversationAsync(@event.ConversationId, "MemberJoined", payload,cancellationToken);

            // Notify the joining user.
            await realtimeService.SendToUserAsync(@event.UserId,"JoinedConversation",payload,cancellationToken);
        }
    }
}
