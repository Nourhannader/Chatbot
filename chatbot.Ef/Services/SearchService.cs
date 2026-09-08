using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Repositories;

namespace chatbot.Ef.Services
{
    public class SearchService(IUnitOfWork unitOfWork,IMapper mapper) : ISearchService
    {
        private async Task<SearchResultDto> SearchUserAsync(SearchDto dto)
        {
            var result= await unitOfWork.Searches.SearchUsersAsync(dto.Keyword,dto.PageNumber,dto.PageSize);
            return new SearchResultDto
            {
                Type = SearchType.Users,

                TotalCount = result.TotalCount,

                PageNumber = dto.PageNumber,

                PageSize = dto.PageSize,

                Data = mapper.Map<
            List<UserSearchDto>>(result.Items)
            };
        }

        private async Task<SearchResultDto>SearchConversationsAsync(Guid userId,SearchDto dto)
        {
            var result =
                await unitOfWork.Searches
                    .SearchConversationsAsync(userId,dto.Keyword,dto.PageNumber,dto.PageSize);

            return new SearchResultDto
            {
                Type = SearchType.Conversations,

                TotalCount = result.TotalCount,

                PageNumber = dto.PageNumber,

                PageSize = dto.PageSize,

                Data = mapper.Map<
                    List<ConversationSearchDto>>(
                        result.Items)
            };
        }

        private async Task<SearchResultDto> SearchMessagesAsync(Guid userId,SearchDto dto)
        {
            var result = await unitOfWork.Searches
                    .SearchMessagesAsync(userId,dto.Keyword,dto.PageNumber,dto.PageSize,dto.MessageType,dto.From,dto.To);

            return new SearchResultDto
            {
                Type = SearchType.Messages,

                TotalCount = result.TotalCount,

                PageNumber = dto.PageNumber,

                PageSize = dto.PageSize,

                Data = mapper.Map<
                    List<MessageSearchDto>>(
                        result.Items)
            };
        }
        
        private async Task<SearchResultDto> SearchFilesAsync(Guid userId,SearchDto dto)
        {
            var result =
                await unitOfWork.Searches
                    .SearchFilesAsync(userId, dto.Keyword, dto.PageNumber, dto.PageSize);

            return new SearchResultDto
            {
                Type = SearchType.Messages,

                TotalCount = result.TotalCount,

                PageNumber = dto.PageNumber,

                PageSize = dto.PageSize,

                Data = mapper.Map<
                    List<FileSearchDto>>(
                        result.Items)
            };
        }
        public async Task<SearchResultDto> SearchAsync(Guid userId, SearchDto dto)
        {
            if (dto.PageNumber < 1)
                dto.PageNumber = 1;
            if (dto.PageSize < 1)
                dto.PageSize = 20;
            if (dto.PageSize > 100)
                dto.PageSize = 100;
            return dto.SearchType switch
            {
                SearchType.Users =>
                    await SearchUserAsync(dto),
                SearchType.Conversations => 
                    await SearchConversationsAsync(userId,dto),
                SearchType.Messages =>
                    await SearchMessagesAsync(userId,dto),
                SearchType.Files =>
                       await SearchFilesAsync(userId,dto),

                _ => throw new ArgumentException("Invalid search type")
            };
        }
    }
}
