using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Media;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Events.Handlers.Media
{
    public sealed class MediaProcessedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<MediaProcessedEvent>
    {
        public async Task HandleAsync(MediaProcessedEvent @event,CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.MessageId,
                @event.StoredFileId,
                @event.Width,
                @event.Height,
                @event.DurationSeconds,
                @event.ThumbnailFileId,
                @event.ProcessedAt
            };

            await realtimeService.SendToConversationAsync(@event.ConversationId,"MediaProcessed", payload,cancellationToken);
        }
    }
}
