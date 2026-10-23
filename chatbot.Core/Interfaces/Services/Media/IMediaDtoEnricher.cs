using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services.Media
{
    public interface IMediaDtoEnricher
    {
        Task EnrichAsync(Message message, MessageDto dto, CancellationToken cancellationToken = default);
    }
}
