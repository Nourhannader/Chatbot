using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<GroupDto> CreateAsync(
        Guid ownerId,
        CreateGroupDto dto);

        Task AddMemberAsync(
            Guid conversationId,
            Guid currentUserId,
            Guid newUserId);

        Task RemoveMemberAsync(
            Guid conversationId,
            Guid currentUserId,
            Guid memberId);

        Task PromoteAsync(
            Guid conversationId,
            Guid currentUserId,
            Guid memberId);

        Task DemoteAsync(
            Guid conversationId,
            Guid currentUserId,
            Guid memberId);

        Task LeaveAsync(
            Guid conversationId,
            Guid userId);

        Task TransferOwnershipAsync(
            Guid conversationId,
            Guid ownerId,
            Guid newOwnerId);

        Task UpdateAsync(
            Guid conversationId,
            Guid userId,
            UpdateGroupDto dto);

        Task DeleteAsync(
            Guid conversationId,
            Guid userId);
    }
}
