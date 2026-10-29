using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Events.Handlers.Messages
{
    public sealed class MessageCreatedSignalRHandler(IChatRealtimeService realtimeService,IMapper mapper,IUnitOfWork unitOfWork) : IEventHandler<MessageCreatedEvent>
    {
        public async Task HandleAsync( MessageCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            var message = await unitOfWork.Messages
             .GetWithDetailsAsync(@event.MessageId);

            if (message is null)
                return;

            var dto = mapper.Map<MessageDto>(message);

            await realtimeService.MessageSentAsync(@event.ConversationId, dto);
        }
    }
}
