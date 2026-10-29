using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using MimeKit;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageDeletedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MessageDeletedEvent>
    {
        public async Task HandleAsync(MessageDeletedEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.DeleteType == MessageDeleteType.ForEveryone)
            {
                var deleteMessageDto = new DeleteMessageResultDto
                {
                    MessageId = @event.MessageId,
                    ConversationId = @event.ConversationId,
                    DeletedByUserId = @event.UserId,
                    DeletedAt = @event.DeletedAt
                };
                await realtimeService.MessageDeletedAsync(@event.ConversationId, "MessageDeleted", deleteMessageDto);

                return;
            }
            var messageDeletedForUserDto = new MessageDeletedForUserDto
            {
                MessageId = @event.MessageId,
                ConversationId = @event.ConversationId,
                DeletedAt = @event.DeletedAt
            };

            await realtimeService.MessageDeletedForUserAsync(@event.UserId, messageDeletedForUserDto);
        }
    }
}
