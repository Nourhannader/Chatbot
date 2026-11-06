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
    public sealed class VoiceNoteProcessedSignalRHandler(IChatRealtimeService realtimeService) : IEventHandler<VoiceNoteProcessedEvent>
    {
        public async Task HandleAsync( VoiceNoteProcessedEvent @event, CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                @event.MessageId,
                @event.VoiceNoteId,
                @event.DurationSeconds,
                @event.Waveform,
                @event.ProcessedAt
            };

            await realtimeService.SendToConversationAsync(@event.ConversationId, "VoiceNoteProcessed", payload,cancellationToken);
        }
    }
}
