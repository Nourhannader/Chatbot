using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Messages
{
    public sealed record MessageReplyCreatedEvent(Guid MessageId, Guid ReplyToMessageId ,
            Guid ConversationId , Guid SenderId, Guid OriginalMessageSenderId , string? Content , DateTime SentAt ):DomainEvent;
    
}
