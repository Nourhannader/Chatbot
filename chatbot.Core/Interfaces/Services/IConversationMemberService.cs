using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface IConversationMemberService
    {
        Task<List<ConversationMemberDto>> GetMembersAsync(ClaimsPrincipal user, Guid conversationId);

        Task<ConversationMemberDetailsDto> GetCurrentMemberAsync(ClaimsPrincipal user, Guid conversationId);

        Task MarkAsReadAsync(ClaimsPrincipal user, Guid conversationId, Guid? lastReadMessageId = null);
    }
}
