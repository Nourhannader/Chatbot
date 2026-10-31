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
            throw new NotImplementedException();
        }

        public Task IndexAsync(MessageSearchDocument document)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultDto<MessageSearchResultDto>> SearchAsync( Guid userId,MessageSearchQuery request)
        {
            return await unitOfWork.Searches.SearchMessagesAsync(userId, request);
        }

        public Task UpdateContentAsync(Guid messageId, string content)
        {
            throw new NotImplementedException();
        }
    }
}
