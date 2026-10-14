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
    public interface IConversationSettingsService
    {
        Task<ConversationSettingsDto> GetAsync(ClaimsPrincipal user, Guid conversationId);
        Task MuteAsync(ClaimsPrincipal user,Guid conversationId,DateTime? muteUntil);

        Task UnmuteAsync(ClaimsPrincipal user,Guid conversationId);

        Task ArchiveAsync(ClaimsPrincipal user,Guid conversationId);

        Task UnarchiveAsync(ClaimsPrincipal user,Guid conversationId);
        Task PinAsync(ClaimsPrincipal user, Guid conversationId);

        Task UnPinAsync(ClaimsPrincipal user, Guid conversationId);
    }
}
