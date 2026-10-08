using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageRead;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMessageReadService
    {
        Task<MessageReadDto> MarkAsReadAsync(Guid messageId,Guid userId);

        Task<IEnumerable<MessageReadDto>> GetMessageReadersAsync(Guid messageId);

        Task<bool> HasUserReadMessageAsync(Guid messageId,Guid userId);
    }
}
