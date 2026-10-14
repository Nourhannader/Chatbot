using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IConversationService
    {
     
        //private conversation
        Task<ConversationDetailsDto> CreateConversationAsync(Guid creatorId, Guid secondUserId);
        Task<List<ConversationDto>> GetUserConversationsAsync(Guid userId);
        Task<ConversationDetailsDto>GetDetailsAsync(ClaimsPrincipal user,Guid conversationId);
    }
}
