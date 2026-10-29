using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageCreatedNotificationHandler(INotificationService notificationService,IUnitOfWork unitOfWork) : IEventHandler<MessageCreatedEvent>
    {
        public async Task HandleAsync( MessageCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            var recipientIds =await unitOfWork.ConversationMember.GetOtherMemberIdsAsync( @event.ConversationId,@event.SenderId);

            foreach (var recipientId in recipientIds)
            {
                await notificationService.SendAsync( recipientId, NotificationType.NewMessage,"New message",
                    @event.Content ?? "New message",
                    new NotificationOptions
                    {
                        ConversationId = @event.ConversationId,
                        MessageId = @event.MessageId
                    });
            }
        }
    }
}
