using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.BackgroundJobs
{
    public interface IMediaProcessingJob
    {
        Task ProcessAttachmentAsync(Guid storedFileId);

        Task ProcessVoiceNoteAsync(Guid voiceNoteId);
    }
}
