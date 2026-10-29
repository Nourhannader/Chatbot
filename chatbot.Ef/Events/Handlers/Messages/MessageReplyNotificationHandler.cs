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
    public sealed class MessageReplyNotificationHandler(INotificationService notificationService,IUnitOfWork unitOfWork): IEventHandler<MessageCreatedEvent>
    {
        public async Task HandleAsync( MessageCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            if (!@event.ReplyToMessageId.HasValue)
                return;

            var original =await unitOfWork.Messages.GetByIdAsync(@event.ReplyToMessageId.Value);

            if (original is null)
                return;

            if (original.SenderId == @event.SenderId)
                return;

            await notificationService.SendAsync( original.SenderId, NotificationType.MessageReply, "New reply","Someone replied to your message.",
            new NotificationOptions
            {
                ConversationId = @event.ConversationId,
                MessageId =@event.MessageId
            });
        }
    }
}
