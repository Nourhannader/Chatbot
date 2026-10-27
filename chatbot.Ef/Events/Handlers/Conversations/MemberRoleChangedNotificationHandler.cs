using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class MemberRoleChangedNotificationHandler(INotificationService notificationService) : IEventHandler<MemberRoleChangedEvent>
    {
        public async Task HandleAsync(MemberRoleChangedEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.UserId ==
                @event.ChangedByUserId)
            {
                return;
            }

            await notificationService.CreateRoleChangedNotificationAsync(
                @event.ConversationId,
                @event.UserId,
                @event.ChangedByUserId,
                @event.OldRole,
                @event.NewRole,
                cancellationToken);
        }
    }
}
