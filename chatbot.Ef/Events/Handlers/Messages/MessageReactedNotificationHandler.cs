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
    public sealed class MessageReactedNotificationHandler(IUnitOfWork unitOfWork, INotificationService notificationService): IEventHandler<MessageReactedEvent>
    {
        public async Task HandleAsync(MessageReactedEvent @event,CancellationToken cancellationToken = default)
        {
            if (@event.IsRemoved)
                return;

            var message =await unitOfWork.Messages.GetByIdAsync( @event.MessageId);

            if (message is null || message.IsDeletedForEveryone)
            {
                return;
            }

            if (message.SenderId == @event.UserId)
                return;

            await notificationService.SendAsync(
            message.SenderId, NotificationType.MessageReaction, "New reaction",
            "Someone reacted to your message.",
            new NotificationOptions
            {
                ConversationId = @event.ConversationId,
                MessageId = @event.MessageId
            });
        }
    }
}
