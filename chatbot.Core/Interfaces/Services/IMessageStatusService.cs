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
        Task MarkDeliveredAsync(string messageId, string recipientId);
        Task MarkReadAsync(string messageId, string recipientId);
        Task<MessageStatus> GetStatusAsync(string messageId, string recipientId);
        Task<List<MessageRecipientStatus>> GetStatusesAsync(string messageId);
    }
}
