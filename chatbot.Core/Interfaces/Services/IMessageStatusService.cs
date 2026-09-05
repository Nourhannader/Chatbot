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
        Task MarkASSentAsync(Guid messageId, Guid recipientId);
        Task MarkAsDeliveredAsync(Guid messageId, Guid recipientId);
        Task MarkAsReadAsync(Guid messageId, Guid recipientId);
        Task<MessageStatus?> GetStatusAsync(Guid messageId, Guid recipientId);
    }
}
