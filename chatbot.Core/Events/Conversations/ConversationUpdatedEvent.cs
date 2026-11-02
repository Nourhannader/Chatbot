using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Conversations
{
    public sealed record ConversationUpdatedEvent( Guid ConversationId,Guid UpdatedByUserId,
    string Title, string? Description, DateTime UpdatedAt) : DomainEvent;
}
