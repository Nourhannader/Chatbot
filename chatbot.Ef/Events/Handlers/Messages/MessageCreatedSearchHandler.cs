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
    public sealed class MessageCreatedSearchHandler(ISearchService searchService) : IEventHandler<MessageCreatedEvent> //IMessageSearchIndexer searchIndexer
    {
        public async Task HandleAsync(MessageCreatedEvent @event,CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(@event.Content))
                return;

            await searchService.IndexAsync(
                @event.MessageId,
                @event.ConversationId,
                @event.SenderId,
                @event.Content,
                @event.SentAt,
                cancellationToken);
        }
    }
}
