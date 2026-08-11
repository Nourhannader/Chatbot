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

        Task<List<Message>> SearchMessagesAsync(string conversationId,string keyword);

        Task<List<Conversation>> SearchConversationsAsync(string userId,string keyword);

        Task<List<Message>> SearchFilesAsync(string conversationId,string keyword);
    }
}
