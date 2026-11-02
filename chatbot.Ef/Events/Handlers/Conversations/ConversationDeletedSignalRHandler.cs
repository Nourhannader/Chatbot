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
    public sealed class ConversationDeletedSignalRHandler(IUnitOfWork unitOfWork,IChatRealtimeService realtimeService) : IEventHandler<ConversationDeletedEvent>
    {
        public async Task HandleAsync(ConversationDeletedEvent @event,CancellationToken cancellationToken = default)
        {
            // The conversation is soft-deleted, but memberships
            // are still available for notifying its members.
            var memberIds = await unitOfWork.ConversationMember.GetActiveMemberIdsAsync(@event.ConversationId);

            var payload = new
            {
                @event.ConversationId,
                @event.DeletedByUserId,
                @event.DeletedAt
            };

            foreach (var memberId in memberIds)
            {
                await realtimeService.SendToUserAsync(memberId, "ConversationDeleted",payload, cancellationToken);
            }
        }
    }
}
