using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IVoiceNoteRepository:IBaseRepository<VoiceNote,Guid>
    {
        Task<VoiceNote?> GetByMessageIdAsync(Guid messageId);

        Task<VoiceNote?> GetByFileIdAsync( Guid fileId);

        Task<VoiceNote?> GetForProcessingAsync(Guid voiceNoteId);

        void Remove(VoiceNote voiceNote);
    }
}
