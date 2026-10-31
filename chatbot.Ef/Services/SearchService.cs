using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Search;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Search;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Repositories;

namespace chatbot.Ef.Services
{
    public class SearchService(IUnitOfWork unitOfWork, IMessageSearchIndex messageSearchIndex) : ISearchService
    {
        public async Task<PagedResultDto<SearchResultDto>> SearchAsync(Guid userId, SearchRequestDto request)
        {
            return request.Type switch
            {
                SearchType.Users =>
                    await unitOfWork.Searches.SearchUsersAsync(userId, request),

                SearchType.Conversations =>
                    await unitOfWork.Searches.SearchConversationsAsync(userId, request),
                SearchType.Messages =>
                    await SearchMessagesAsync(userId, request),
                SearchType.Files =>
                    await unitOfWork.Searches.SearchFilesAsync(userId, request),

                SearchType.Groups =>
                    await unitOfWork.Searches.SearchGroupsAsync(userId, request),

                SearchType.All =>
                     throw new NotImplementedException("Search All will use a separate aggregated response."),

                _ => throw new ArgumentOutOfRangeException(nameof(request.Type), "Invalid search type.")
            };
        }

        private async Task<PagedResultDto<SearchResultDto>> SearchMessagesAsync(Guid userId, SearchRequestDto request)
        {
            var messageQuery =new MessageSearchQuery
                {
                    Keyword =request.Keyword,
                    ConversationId = request.ConversationId,
                    SenderId = request.SenderId,
                    MessageType =request.MessageType,
                    From = request.From,
                    To =request.To,
                    Page = request.Page,
                    PageSize =request.PageSize
                };


            var result = await messageSearchIndex.SearchAsync( userId, messageQuery);


            return new PagedResultDto<SearchResultDto>
            {
                Items = result.Items
                    .Select(message =>
                        new SearchResultDto
                        {
                            Type = SearchType.Messages,
                            Id =message.MessageId,
                            ConversationId =message.ConversationId,
                            Title = message.SenderName ?? string.Empty,
                            Description = message.Content,
                            CreatedAt = message.CreatedAt
                        })
                    .ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount =result.TotalCount
            };
        }


    }
}
