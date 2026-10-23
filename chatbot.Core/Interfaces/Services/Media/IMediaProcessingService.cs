using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services.Media
{
    public interface IMediaProcessingService
    {
        Task ProcessAttachmentAsync(Guid storedFileId,CancellationToken cancellationToken = default);

        Task ProcessVoiceNoteAsync(Guid voiceNoteId,CancellationToken cancellationToken = default);
    }
}
