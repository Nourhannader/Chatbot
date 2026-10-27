using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class ConversationCreatedSignalRHandler(IConversationMemberRepository memberRepository, IChatRealtimeService realtimeService) : IEventHandler<ConversationCreatedEvent>
    {
        public async Task HandleAsync(ConversationCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            var memberIds = await memberRepository.GetActiveMemberIdsAsync( @event.ConversationId);

            foreach (var userId in memberIds)
            {
                await realtimeService.SendToUserAsync(userId,"ConversationCreated",
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
