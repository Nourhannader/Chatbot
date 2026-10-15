using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<ConversationDetailsDto> CreateAsync(ClaimsPrincipal user, CreateGroupDto dto);

        Task AddMemberAsync(ClaimsPrincipal user, Guid conversationId,Guid userId);

        Task RemoveMemberAsync(ClaimsPrincipal user, Guid conversationId,Guid memberId);

        Task PromoteAsync(ClaimsPrincipal user, Guid conversationId,Guid memberId);

        Task DemoteAsync(ClaimsPrincipal user, Guid conversationId,Guid memberId);

        Task LeaveAsync(ClaimsPrincipal user,Guid conversationId);

        Task TransferOwnershipAsync(ClaimsPrincipal user, Guid conversationId,Guid newOwnerId);

        Task<ConversationDetailsDto> UpdateAsync(ClaimsPrincipal user, Guid conversationId, UpdateGroupDto dto);

        Task DeleteAsync(ClaimsPrincipal user, Guid conversationId);
        
        Task BanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId,BanMemberDto dto );
        
        Task UnBanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId);
        
        Task MuteMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid userId, MuteConversationDto dto);
        
        Task UnMuteMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid userId);
    }
}
