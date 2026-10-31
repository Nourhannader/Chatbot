using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Search;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageDeletedSearchHandler(IMessageSearchIndex searchIndex) : IEventHandler<MessageDeletedEvent>
    {
        public async Task HandleAsync( MessageDeletedEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.DeleteType != MessageDeleteType.ForEveryone)
            {
                return;
            }

            await searchIndex.DeleteAsync( @event.MessageId);
        }
    }
}
