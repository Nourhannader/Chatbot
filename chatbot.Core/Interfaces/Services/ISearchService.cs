using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface ISearchService
    {
        Task<List<UserDto>> SearchUsersAsync(string keyword);

        Task<List<MessageDto>> SearchMessagesAsync( Guid conversationId,string keyword);

        Task<List<ConversationDto>> SearchConversationsAsync(Guid userId,string keyword);

        Task<List<MessageDto>> SearchFilesAsync(Guid conversationId,string keyword);
    }
}
