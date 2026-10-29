using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageRecipientStatusChangedSignalRHandler(IUnitOfWork unitOfWork,IChatRealtimeService realtimeService) : IEventHandler<MessageRecipientStatusChangedEvent>
    {
        public async Task HandleAsync( MessageRecipientStatusChangedEvent @event, CancellationToken cancellationToken = default)
        {
            var recipient = await unitOfWork.MessageStatuses.GetAsync(@event.MessageId, @event.UserId);

            if (recipient is null)
                return;

            var dto = new MessageRecipientStatusDto
            {
                MessageId = recipient.MessageId,
                UserId = recipient.RecipientId,
                UserName = recipient.Recipient?.UserName ?? string.Empty,
                Status = recipient.Status,
                SentAt = recipient.SentAt,
                DeliveredAt = recipient.DeliveredAt,
                ReadAt = recipient.ReadAt
            };

            await realtimeService.MessageDeliveredAsync(@event.ConversationId, dto);
        }
    }
}
