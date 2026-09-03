using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMessageService
    {
        public Task SendMessageAsync(Guid conversationId,Guid senderId,string? content,
           IEnumerable<IFormFile>? files,
           CancellationToken cancellationToken = default);
        Task<PagedResultDto<Message>> GetMessagesAsyns(Guid conversationId, int page, int pageSize);
        Task DeleteForEveryoneAsync(Guid messageId,Guid userId);
        Task MarkDeliveredAsync(Guid messageId, Guid userId);
        Task MarkReadAsync(Guid conversationId, Guid userId);
        Task<Message> ReplyAsync(Guid senderId, ReplyMessageDto dto);
        Task<MessageDto> SendFileAsync(SendFileDto filedto);
    }
}
