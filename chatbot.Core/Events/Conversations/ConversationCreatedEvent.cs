using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Conversations
{
    public sealed record ConversationCreatedEvent(Guid ConversationId,Guid CreatedByUserId,
    ConversationType ConversationType, DateTime CreatedAt ) : DomainEvent;
}
