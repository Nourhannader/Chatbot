using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMessageStatusService
    {
        Task<MessageStatus?> GetStatusAsync(Guid messageId, Guid recipientId);

        Task MarkAsSentAsync(Guid messageId, Guid recipientId);

        Task MarkAsDeliveredAsync(Guid messageId, Guid recipientId);

        Task MarkAsReadAsync(Guid messageId, Guid recipientId);

        Task<List<MessageRecipientStatusDto>> GetByMessageAsync(Guid messageId);

        Task<List<MessageRecipientStatusDto>> GetReadersAsync(Guid messageId);

        Task<bool> HasUserReadMessageAsync(Guid messageId, Guid recipientId);
    }
}
