using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Messages
{
    public sealed record MessageForwardedEvent(Guid MessageId, Guid OriginalMessageId,
       Guid SourceConversationId, Guid TargetConversationId, Guid ForwardedByUserId, DateTime ForwardedAt) : DomainEvent;
}
