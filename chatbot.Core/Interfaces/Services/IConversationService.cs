using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services
{
    public interface IConversationService
    {
        Task<ConversationDetailsDto> CreatePrivateAsync(ClaimsPrincipal user, CreatePrivateConversationDto dto);

        Task<List<ConversationDto>> GetMineAsync(ClaimsPrincipal user);

        Task<List<ConversationDto>> GetMyConversationsAsync(ClaimsPrincipal user);

        Task<ConversationDetailsDto> GetDetailsAsync(ClaimsPrincipal user, Guid conversationId);

        Task<ConversationImageDto> AddOrUpdateImageAsync(ClaimsPrincipal user,Guid conversationId,UpdateConversationImageDto dto);

        Task RemoveImageAsync(ClaimsPrincipal user, Guid conversationId);
    }
}
