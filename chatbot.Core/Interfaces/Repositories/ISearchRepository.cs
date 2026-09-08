using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface ISearchRepository
    {
        Task<(List<ApplicationUser>Items,int TotalCount)> SearchUsersAsync(string keyword,int pageNumber,int pageSize);

        Task<(List<Message> Items, int TotalCount)> SearchMessagesAsync(Guid UserId,string keyword,int pageNumber, int pageSize
            ,MessageType? messageType,DateTime? from,DateTime? to);

        Task<(List<Conversation> Items, int TotalCount)> SearchConversationsAsync(Guid userId, string keyword, int pageNumber, int pageSize);

        Task<(List<StoredFile> Items, int TotalCount)> SearchFilesAsync(Guid userId, string keyword, int pageNumber, int pageSize);
    }
}
