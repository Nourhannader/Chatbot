using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Search;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface ISearchRepository
    {
        Task<PagedResultDto<SearchResultDto>> SearchUsersAsync(Guid currentUserId,SearchRequestDto request);

        Task<PagedResultDto<SearchResultDto>> SearchConversationsAsync(Guid currentUserId,SearchRequestDto request);

        Task<PagedResultDto<MessageSearchResultDto>> SearchMessagesAsync(Guid userId, MessageSearchQuery request);

        Task<PagedResultDto<SearchResultDto>> SearchFilesAsync(Guid currentUserId,SearchRequestDto request);

        Task<PagedResultDto<SearchResultDto>> SearchGroupsAsync(Guid currentUserId, SearchRequestDto request);
    }
}
