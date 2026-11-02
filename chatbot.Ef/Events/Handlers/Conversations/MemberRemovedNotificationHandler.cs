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
    public sealed class MemberRemovedNotificationHandler( INotificationService notificationService) : IEventHandler<MemberRemovedEvent>
    {
        public async Task HandleAsync( MemberRemovedEvent @event,CancellationToken cancellationToken = default)
        {
            if (@event.RemovalType == MemberRemovalType.Left)
                return;

            var (type, title, body) = @event.RemovalType switch
            {
                MemberRemovalType.Removed => (NotificationType.MemberRemoved,"Removed from group", "You have been removed from a group."),

                MemberRemovalType.Banned => (NotificationType.MemberBanned, "Banned from group","You have been banned from a group."),

                _ => throw new ArgumentOutOfRangeException()
            };

            await notificationService.SendAsync(@event.UserId,type,title,body, @event.ConversationId, cancellationToken: cancellationToken);
        }
    }
}
