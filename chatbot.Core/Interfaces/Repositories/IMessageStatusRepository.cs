using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IMessageStatusRepository
    {
        Task AddRangeAsync(IEnumerable<MessageRecipientStatus> recipients);

        Task<MessageRecipientStatus?> GetAsync(Guid messageId, Guid userId);

        Task<List<MessageRecipientStatus>> GetByMessageIdAsync(Guid messageId);

        Task<List<MessageRecipientStatus>> GetUnreadAsync(Guid userId, Guid conversationId, DateTime until);

        Task<bool> ExistsAsync(Guid messageId, Guid userId);

        Task<MessageStatusSummaryDto?> GetSummaryAsync(Guid messageId);
    }
}
