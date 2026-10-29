using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessagesReadSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MessagesReadEvent>
    {
        public async Task HandleAsync(MessagesReadEvent @event, CancellationToken cancellationToken = default)
        {
            var dto = new MessagesReadEventDto
            {
                UserId = @event.UserId,
                ConversationId = @event.ConversationId,
                LastMessageId = @event.LastMessageId,
                ReadAt = @event.ReadAt
            };

            await realtimeService.MessagesReadAsync(@event.ConversationId, dto);
        }
    }
}
