using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Services
{
    public class SearchService(IUnitOfWork unitOfWork) : ISearchService
    {
        public async Task<List<ConversationDto>> SearchConversationsAsync(Guid userId, string keyword)
        {
            var conversations=await unitOfWork.Searches.SearchConversationsAsync(userId, keyword);
            return conversations.Select(
                c => new ConversationDto
                {
                    Id=c.Id.ToString(),
                    Title=c.Title,
                    Type=c.Type,
                    GroupPictureUrl=c.GroupPictureUrl,
                    CreatedAt=c.CreatedAt
                }
                ).ToList();
        }

        public async Task<List<MessageDto>> SearchFilesAsync(Guid conversationId, string keyword)
        {
            var files = await unitOfWork.Searches.SearchFilesAsync(conversationId, keyword);
            return files.Select(
                f => new MessageDto
                {
                    Id = f.Id.ToString(),
                    FileUrl = f.FileUrl,
                    SenderId = f.SenderId,
                    Type = f.Type,
                    Content = f.Content
                }
                ).ToList();

        }

        public async Task<List<MessageDto>> SearchMessagesAsync(Guid conversationId, string keyword)
        {
            var messages = await unitOfWork.Searches.SearchMessagesAsync(conversationId, keyword);
            return messages.Select(
               m => new MessageDto
               {
                   Id = m.Id.ToString(),
                   FileUrl = m.FileUrl,
                   SenderId = m.SenderId.ToString(),
                   Type = m.Type,
                   Content = m.Content
               }
               ).ToList();
        }

        public async Task<List<UserDto>> SearchUsersAsync(string keyword)
        {
            var users = await unitOfWork.Searches.SearchUsersAsync(keyword);
            return users.Select(
               u => new UserDto
               {
                   Id = u.Id.ToString()
               }
               ).ToList();
        }
    }
}
