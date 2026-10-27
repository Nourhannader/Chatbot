using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Messages
{
    public sealed record MessageRecipientStatusChangedEvent(Guid MessageId, Guid ConversationId,
    Guid UserId,MessageStatus Status, DateTime StatusChangedAt ) : DomainEvent;
}
