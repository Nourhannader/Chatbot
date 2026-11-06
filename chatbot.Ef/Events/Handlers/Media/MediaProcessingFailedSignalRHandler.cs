using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Events.Media;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Events.Handlers.Media
{
    public sealed class MediaProcessingFailedSignalRHandler( IChatRealtimeService realtimeService) : IEventHandler<MediaProcessingFailedEvent>
    {
        public Task HandleAsync( MediaProcessingFailedEvent @event, CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.MessageId,
                @event.StoredFileId,
                Status = FileProcessingStatus.Failed.ToString(),
                @event.Error,
                @event.FailedAt
            };

            return realtimeService.SendToConversationAsync(@event.ConversationId,"MediaProcessingFailed",payload,cancellationToken);
        }
    }
}
