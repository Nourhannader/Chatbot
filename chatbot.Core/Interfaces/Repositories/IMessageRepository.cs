using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IMessageRepository : IBaseRepository<Message,Guid>
    {
        Task AddDeletionAsync(MessageDeletion message);
        Task<Message?> GetMessageByConversationIdAsync(Guid messageId, Guid conversationId);
        Task<Message?> GetWithFilesAsync(Guid id);
        Task<Message?> GetWithDetailsAsync(Guid messageId);
        Task<List<Message>> GetConversationMessagesAsync( Guid conversationId,Guid userId,int page,int pageSize);
        Task<int> CountByConversationAsync(Guid conversationId,Guid userId);
        Task<List<Message>> SearchMessagesAsync(Guid conversationId,Guid userId ,string keyword);
        Task<bool> IsDeletedForUserAsync(Guid messageId,Guid userId);
        Task<List<Message>> GetAllMessageDeletedOlder(DateTime cutoff);
       
        void Remove(Message message);
        void RemoveRange(List<Message> messages);

    }
}
