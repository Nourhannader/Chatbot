using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class ConversationCreatedSignalRHandler(IUnitOfWork unitOfWork, IChatRealtimeService realtimeService) : IEventHandler<ConversationCreatedEvent>
    {
        public async Task HandleAsync(ConversationCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            var memberIds = await unitOfWork.ConversationMember
             .GetActiveMemberIdsAsync(@event.ConversationId);

            foreach (var userId in memberIds)
            {
                await realtimeService.SendToUserAsync(
                    userId,
                    "ConversationCreated",
                    new
                    {
                        @event.ConversationId,
                        @event.CreatedByUserId,
                        @event.ConversationType,
                        @event.CreatedAt
                    },
                    cancellationToken);
            }
        }
    }
}
