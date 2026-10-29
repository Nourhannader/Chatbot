using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageEditedSignalRHandler(IChatRealtimeService realtimeService): IEventHandler<MessageEditedEvent>
    {
        public async Task HandleAsync( MessageEditedEvent @event, CancellationToken cancellationToken = default)
        {
            var dto = new MessageEditedDto
            {
                MessageId =@event.MessageId,
                Content = @event.Content,
                Version = @event.Version,
                EditedAt =@event.EditedAt
            };

            await realtimeService.MessageEditedAsync( @event.ConversationId, dto);
        }
    }
}
