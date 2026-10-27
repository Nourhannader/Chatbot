using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using Microsoft.VisualBasic.FileIO;

namespace chatbot.Core.Interfaces.Services
{
    public interface ISearchService
    {
        Task DeleteAsync(Guid messageId, CancellationToken cancellationToken);
        Task IndexAsync(Guid messageId, Guid conversationId, Guid senderId, string content, DateTime sentAt, CancellationToken cancellationToken);
        Task<SearchResultDto> SearchAsync(Guid userId, SearchDto dto);
        Task UpdateAsync(Guid messageId, string content, CancellationToken cancellationToken);
    }
}
