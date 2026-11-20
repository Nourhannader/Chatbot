using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.BackgroundJobs;
using chatbot.Core.Interfaces.Services.Media;
using chatbot.Core.Models;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    [Queue("media")]
    [AutomaticRetry(Attempts = 3)]
    public  sealed class MediaProcessingJob(IMediaProcessingService mediaService,ILogger<MediaProcessingJob> logger) : IMediaProcessingJob
    {

        public async Task ProcessAttachmentAsync(Guid storedFileId)
        {
            logger.LogInformation("Processing attachment {AttachmentId}", storedFileId);

            await mediaService.ProcessAttachmentAsync(storedFileId);

            logger.LogInformation("Attachment {AttachmentId} processed", storedFileId);
        }

        public async Task ProcessVoiceNoteAsync(Guid voiceNoteId)
        {
            logger.LogInformation("Processing voicNote {voiceNoteId}", voiceNoteId);

            await mediaService.ProcessVoiceNoteAsync(voiceNoteId);

            logger.LogInformation("voicNote {voiceNoteId} processed", voiceNoteId);
        }
    }
}
