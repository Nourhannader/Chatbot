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
    public sealed class MemberAddedNotificationHandler( INotificationService notificationService) : IEventHandler<MemberAddedEvent>
    {
        public async Task HandleAsync(
            MemberAddedEvent @event,
            CancellationToken cancellationToken = default)
        {
            if (@event.UserId == @event.AddedByUserId)
                return;

            await notificationService.CreateMemberAddedNotificationAsync(
                @event.ConversationId,
                @event.UserId,
                @event.AddedByUserId,
                cancellationToken);
        }
    }
}
