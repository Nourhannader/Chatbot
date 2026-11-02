using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Search;
using chatbot.Core.Interfaces.Search;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Search.Providers
{
    public class SqlMessageSearchIndex(IUnitOfWork unitOfWork) : IMessageSearchIndex
    {
        public Task DeleteAsync(Guid messageId)
        {
            // No separate index exists.
            // SearchAsync excludes deleted messages.
            return Task.CompletedTask;
        }

        public Task IndexAsync(MessageSearchDocument document)
        {
            // Message is already saved in SQL Server.
            // No separate search index needs updating.
            return Task.CompletedTask;
        }

        public async Task<PagedResultDto<MessageSearchResultDto>> SearchAsync( Guid userId,MessageSearchQuery request)
        {
            return await unitOfWork.Searches.SearchMessagesAsync(userId, request);
        }

        public Task UpdateContentAsync(Guid messageId, string content)
        {
            // Message content is already updated in SQL Server.
            return Task.CompletedTask;
        }
    }
}
