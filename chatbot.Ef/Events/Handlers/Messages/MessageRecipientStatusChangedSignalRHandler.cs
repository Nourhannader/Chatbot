using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageRecipientStatusChangedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MessageRecipientStatusChangedEvent>
    {
        public async Task HandleAsync( MessageRecipientStatusChangedEvent @event, CancellationToken cancellationToken = default)
        {
            await realtimeService.SendToConversationAsync(@event.ConversationId, "MessageRecipientStatusChanged",
                new
                {
                    @event.MessageId,
                    @event.UserId,
                    @event.Status,
                    @event.StatusChangedAt
                },
                cancellationToken);
        }
    }
}
