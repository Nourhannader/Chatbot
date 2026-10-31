using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Search;


namespace chatbot.Core.Interfaces.Search
{
    public interface IMessageSearchIndex
    {
        Task IndexAsync( MessageSearchDocument document);

        Task UpdateContentAsync(Guid messageId, string content);

        Task DeleteAsync( Guid messageId);

        Task<PagedResultDto<MessageSearchResultDto>> SearchAsync(Guid userId, MessageSearchQuery query);
    }
}
