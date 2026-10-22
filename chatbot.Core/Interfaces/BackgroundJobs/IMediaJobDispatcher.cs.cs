using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.BackgroundJobs
{
    public interface IMediaJobDispatcher
    {
        void EnqueueAttachmentProcessing(Guid storedFileId);

        void EnqueueVoiceNoteProcessing(Guid voiceNoteId);
    }
}
