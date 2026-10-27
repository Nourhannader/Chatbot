using chatbot.Core.Interfaces.Services.Media;
using Hangfire;

namespace chatbot.Api.BackgroundJobs
{
    public class ProcessAttachmentJob
    {
        private readonly IMediaProcessingService  _processingService;

        public ProcessAttachmentJob(IMediaProcessingService processingService)
        {
            _processingService = processingService;
        }

        [AutomaticRetry(Attempts = 3)]
        public Task ExecuteAsync( Guid storedFileId,CancellationToken cancellationToken)
        {
            return _processingService
                .ProcessAttachmentAsync(storedFileId,cancellationToken);
        }
    }
}
