using chatbot.Core.Interfaces.BackgroundJobs;
using Hangfire;

namespace chatbot.Api.BackgroundJobs
{
    public class HangfireMediaJobDispatcher : IMediaJobDispatcher
    {
        private readonly IBackgroundJobClient _backgroundJobs;

        public HangfireMediaJobDispatcher(IBackgroundJobClient backgroundJobs)
        {
            _backgroundJobs = backgroundJobs;
        }

        public void EnqueueAttachmentProcessing(Guid storedFileId)
        {
            _backgroundJobs.Enqueue<ProcessAttachmentJob>(
                job => job.ExecuteAsync(
                    storedFileId,
                    CancellationToken.None));
        }

        public void EnqueueVoiceNoteProcessing(Guid voiceNoteId)
        {
            _backgroundJobs.Enqueue<ProcessVoiceNoteJob>(
                job => job.ExecuteAsync(
                    voiceNoteId,
                    CancellationToken.None));
        }
    }
}
