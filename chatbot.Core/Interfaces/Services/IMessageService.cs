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
        public Task<MessageDto> SendMessageAsync(Guid senderId,SendMessageDto dto,
           CancellationToken cancellationToken = default);
        Task<PagedResultDto<MessageDto>> GetMessagesAsyns(Guid userId,Guid conversationId, int page=1, int pageSize=20);

        Task<Message> ReplyAsync(Guid senderId, ReplyMessageDto dto);
        Task<MessageDto> SendFileAsync(SendFileDto filedto);
        Task DeleteForMeAsync(Guid messageId,Guid userId);

        Task DeleteForEveryoneAsync( Guid messageId,Guid userId);
    }
}
