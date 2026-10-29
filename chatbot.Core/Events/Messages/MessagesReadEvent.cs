using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Messages
{
    public sealed record MessagesReadEvent(Guid ConversationId,Guid UserId,
    Guid LastMessageId, DateTime ReadAt) : DomainEvent;
}
