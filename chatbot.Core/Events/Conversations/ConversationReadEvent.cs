using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Conversations
{
    public sealed record ConversationReadEvent(
    Guid ConversationId, Guid UserId, DateTime ReadAt) : DomainEvent;
}
