using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMessageStatusService
    {
        Task MarkDeliveredAsync(Guid messageId, Guid recipientId);
        Task MarkReadAsync(Guid messageId, Guid recipientId);
        Task<MessageStatus> GetStatusAsync(Guid messageId, Guid recipientId);
        Task<List<MessageRecipientStatus>> GetStatusesAsync(Guid messageId);
    }
}
