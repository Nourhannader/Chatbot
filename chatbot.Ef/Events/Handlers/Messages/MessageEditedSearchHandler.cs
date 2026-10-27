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
    public sealed class MessageEditedSearchHandler(ISearchService searchService): IEventHandler<MessageEditedEvent>
    {
        public async Task HandleAsync( MessageEditedEvent @event,CancellationToken cancellationToken = default)
        {
            await searchService.UpdateAsync(
                @event.MessageId,
                @event.Content,
                cancellationToken);
        }
    }
}
