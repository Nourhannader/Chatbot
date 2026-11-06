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
    public sealed class MediaProcessingRequestedHandler(IMediaJobDispatcher jobDispatcher) : IEventHandler<MediaProcessingRequestedEvent>
    {
        public Task HandleAsync( MediaProcessingRequestedEvent @event,CancellationToken cancellationToken = default)
        {
            foreach (var fileId in @event.StoredFileIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                jobDispatcher.EnqueueAttachmentProcessing(fileId);
            }

            return Task.CompletedTask;
        }
    }
}
