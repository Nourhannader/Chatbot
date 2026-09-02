using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface ISearchRepository
    {
        Task<List<ApplicationUser>> SearchUsersAsync(string keyword);

        Task<List<Message>> SearchMessagesAsync(Guid conversationId,string keyword);

        Task<List<Conversation>> SearchConversationsAsync(Guid userId,string keyword);

        Task<List<Message>> SearchFilesAsync(Guid conversationId,string keyword);
    }
}
