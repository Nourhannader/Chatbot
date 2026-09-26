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
        Task<Guid> CreateAsync(Guid ownerId,CreateGroupDto dto);

        Task AddMemberAsync(ClaimsPrincipal User, Guid conversationId,Guid userId);

        Task RemoveMemberAsync(ClaimsPrincipal User, Guid conversationId,Guid memberId);

        Task PromoteAsync(ClaimsPrincipal User, Guid conversationId,Guid memberId);

        Task DemoteAsync(ClaimsPrincipal User, Guid conversationId,Guid memberId);

        Task LeaveAsync(Guid conversationId,Guid userId);

        Task TransferOwnershipAsync(ClaimsPrincipal user, Guid conversationId,Guid ownerId,Guid newOwnerId);

        Task UpdateAsync(ClaimsPrincipal user,Guid conversationId,UpdateGroupDto dto);

        Task DeleteAsync(ClaimsPrincipal user, Guid conversationId);
        Task BanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId);
        Task UnBanMemberAsync(ClaimsPrincipal user,Guid conversationId, Guid userId);
        Task MuteMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid userId);
        Task UnMuteMemberAsync(ClaimsPrincipal user, Guid conversationId, Guid userId);
    }
}
