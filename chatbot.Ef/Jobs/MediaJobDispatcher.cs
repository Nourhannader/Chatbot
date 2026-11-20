using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using Hangfire;

namespace chatbot.Ef.Jobs
{
    public sealed class MediaJobDispatcher(IBackgroundJobClient backgroundJobClient) : IMediaJobDispatcher
    {

        public void EnqueueAttachmentProcessing(Guid storedFileId)
        {
            backgroundJobClient.Enqueue<IMediaProcessingJob>(
                job => job.ProcessAttachmentAsync(storedFileId));
        }

        public void EnqueueVoiceNoteProcessing(Guid voiceNoteId)
        {
            backgroundJobClient.Enqueue<IMediaProcessingJob>(
                job => job.ProcessVoiceNoteAsync(voiceNoteId));
        }
    }
}
