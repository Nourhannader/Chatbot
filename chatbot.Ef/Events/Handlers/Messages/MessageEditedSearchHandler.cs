using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Search;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageEditedSearchHandler(IMessageSearchIndex searchIndex): IEventHandler<MessageEditedEvent>
    {
        public async Task HandleAsync( MessageEditedEvent @event,CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(@event.Content))
            {
                await searchIndex.DeleteAsync(@event.MessageId);

                return;
            }

            await searchIndex.UpdateContentAsync( @event.MessageId, @event.Content);
        }
    }
}
