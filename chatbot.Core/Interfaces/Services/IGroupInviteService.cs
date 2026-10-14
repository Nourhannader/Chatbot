using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.InviteGroup;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface IGroupInviteService
    {
        Task<GroupInviteDto> GenerateAsync(ClaimsPrincipal user, Guid conversationId, CreateGroupInviteDto dto);

        Task<List<GroupInviteDto>> GetActiveAsync(ClaimsPrincipal user, Guid conversationId);

        Task RevokeAsync(ClaimsPrincipal user, Guid conversationId, Guid inviteId);

        Task<GroupInvitePreviewDto> GetPreviewAsync(ClaimsPrincipal user, string code);

        Task<ConversationDetailsDto> JoinAsync(ClaimsPrincipal user, string code);
    }
}
