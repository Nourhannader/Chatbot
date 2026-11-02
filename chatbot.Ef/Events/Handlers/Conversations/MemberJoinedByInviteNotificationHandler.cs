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
    public sealed class MemberJoinedByInviteNotificationHandler(INotificationService notificationService) : IEventHandler<MemberJoinedByInviteEvent>
    {
        public async Task HandleAsync(MemberJoinedByInviteEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.InvitedByUserId == @event.UserId)
                return;

            await notificationService.SendAsync(
                @event.InvitedByUserId,
                NotificationType.MemberJoined,
                "Member joined",
                "A user joined your group using an invite.",
                new NotificationOptions
                {
                    ConversationId = @event.ConversationId
                });
        }
    }
}
