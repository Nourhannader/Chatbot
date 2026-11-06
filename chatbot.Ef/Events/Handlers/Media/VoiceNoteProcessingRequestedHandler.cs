using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Media;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Ef.Events.Handlers.Media
{
    public sealed class VoiceNoteProcessingRequestedHandler(IMediaJobDispatcher jobDispatcher)  : IEventHandler<VoiceNoteProcessingRequestedEvent>
    {
        public Task HandleAsync(VoiceNoteProcessingRequestedEvent @event,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            jobDispatcher.EnqueueVoiceNoteProcessing(@event.VoiceNoteId);

            return Task.CompletedTask;
        }
    }
}
