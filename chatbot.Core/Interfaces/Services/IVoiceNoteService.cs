using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface IVoiceNoteService
    {
        Task<MessageDto> SendAsync(
        SendVoiceNoteDto dto,
        Guid userId,
        CancellationToken cancellationToken = default);
    }
}
