using chatbot.Core.Interfaces.Services.Media;
using Hangfire;

namespace chatbot.Api.BackgroundJobs
{
    public class ProcessVoiceNoteJob
    {
        private readonly IMediaProcessingService _processingService;

        public ProcessVoiceNoteJob( IMediaProcessingService processingService)
        {
            _processingService =processingService;
        }

        [AutomaticRetry(Attempts = 3)]
        public Task ExecuteAsync(Guid voiceNoteId,CancellationToken cancellationToken)
        {
            return _processingService
                .ProcessVoiceNoteAsync( voiceNoteId,cancellationToken);
        }
    }
}
