using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageForwardedSignalRHandler(ILogger<MessageForwardedSignalRHandler> logger) : IEventHandler<MessageForwardedEvent>
    {
        public async Task HandleAsync(MessageForwardedEvent @event, CancellationToken cancellationToken = default)
        {
            logger.LogInformation(
            "Message {MessageId} was forwarded from conversation {SourceConversationId} to {TargetConversationId} by user {UserId}.",
            @event.MessageId,
            @event.SourceConversationId,
            @event.TargetConversationId,
            @event.ForwardedByUserId);
        }
    }
}
