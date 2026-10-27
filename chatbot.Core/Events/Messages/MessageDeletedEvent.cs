using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Messages
{
    public sealed record MessageDeletedEvent(Guid MessageId,Guid ConversationId,
    Guid UserId,MessageDeleteType DeleteType, DateTime DeletedAt): DomainEvent;
}
