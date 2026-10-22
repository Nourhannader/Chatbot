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
        Task CreateRecipientsAsync(Message message);

        Task MarkAsDeliveredAsync(Guid userId, Guid messageId);

        Task MarkAsReadAsync(Guid userId, Guid conversationId, Guid lastMessageId);

        Task<List<MessageRecipientStatusDto>> GetReceiptsAsync(Guid messageId, Guid currentUserId);

        Task<MessageStatusSummaryDto> GetSummaryAsync(Guid messageId, Guid currentUserId);

        Task<bool> HasUserReadMessageAsync(Guid messageId, Guid recipientId);
    }
}
