using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Messages
{
    public sealed record MessageEditedEvent(Guid MessageId, Guid ConversationId,
        Guid UserId,string Content, int Version,DateTime EditedAt) : DomainEvent;
    
}
