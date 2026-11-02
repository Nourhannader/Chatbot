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
    public sealed class MemberRoleChangedNotificationHandler(INotificationService notificationService) : IEventHandler<MemberRoleChangedEvent>
    {
        public async Task HandleAsync(MemberRoleChangedEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.OldRole == @event.NewRole)
                return;

            var (type, title, body) = @event.NewRole switch
            {
                ConversationRole.Owner => (NotificationType.OwnershipTransferred,"Group ownership transferred", "You are now the group owner."),

                ConversationRole.Admin => (NotificationType.MemberPromoted,"Promoted to admin", "You have been promoted to group admin."),

                _ => (NotificationType.MemberDemoted, "Admin role removed", "Your group admin role has been removed.")
            };

            await notificationService.SendAsync(@event.UserId, type,title,body,@event.ConversationId,cancellationToken: cancellationToken);
        }
    }
}
