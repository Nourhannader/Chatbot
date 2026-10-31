using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Search;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Search;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageCreatedSearchHandler(IMessageSearchIndex searchIndex) : IEventHandler<MessageCreatedEvent> //IMessageSearchIndexer searchIndexer
    {
        public async Task HandleAsync(MessageCreatedEvent @event,CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(@event.Content))
                return;

            var document = new MessageSearchDocument
            {
                MessageId = @event.MessageId,
                ConversationId = @event.ConversationId,
                SenderId = @event.SenderId,
                Content = @event.Content,
                CreatedAt = @event.SentAt
            };

            await searchIndex.IndexAsync( document);
        }
    }
}
