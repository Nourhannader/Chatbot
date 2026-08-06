using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(string senderId, SendMessageDto messageDto);
        Task<List<Message>> GetMessagesAsyns(string conversationId, int page, int pageSize);
        Task DeleteForEveryoneAsync(string messageId,string userId);
        Task MarkDeliveredAsync( string messageId,string userId);
        Task MarkReadAsync(string conversationId, string userId);
    }
}
