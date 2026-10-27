using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Conversations
{
    public sealed class MemberRemovedNotificationHandler( INotificationService notificationService) : IEventHandler<MemberRemovedEvent>
    {
        public async Task HandleAsync( MemberRemovedEvent @event,CancellationToken cancellationToken = default)
        {
            if (@event.RemovalType == MemberRemovalType.Left)
            {
                return;
            }

            if (@event.RemovedByUserId is null)
                return;

            await notificationService.CreateMemberRemovedNotificationAsync(
                @event.ConversationId,
                @event.UserId,
                @event.RemovedByUserId.Value,
                @event.RemovalType,
                cancellationToken);
        }
    }
}
