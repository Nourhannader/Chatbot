using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class MemberAddedNotificationHandler( INotificationService notificationService) : IEventHandler<MemberAddedEvent>
    {
        public async Task HandleAsync(MemberAddedEvent @event,CancellationToken cancellationToken = default)
        {
            await notificationService.SendAsync(
            @event.UserId,
            NotificationType.MemberAdded,
            "Added to group",
            "You have been added to a group.",
            @event.ConversationId,
            cancellationToken: cancellationToken);
        }
    }
}
