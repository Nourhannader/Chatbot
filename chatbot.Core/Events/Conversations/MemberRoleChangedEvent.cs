using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Conversations
{
    public sealed record MemberRoleChangedEvent( Guid ConversationId, Guid UserId,
    ConversationRole OldRole,ConversationRole NewRole, Guid ChangedByUserId, DateTime ChangedAt) : DomainEvent;
}
