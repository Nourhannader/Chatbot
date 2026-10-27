using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Notifications
{
    public sealed record NotificationCreatedEvent( Guid NotificationId, Guid UserId,
    NotificationType Type, string Title,string Body, Guid? ConversationId, Guid? MessageId,DateTime CreatedAt) : DomainEvent;
}
