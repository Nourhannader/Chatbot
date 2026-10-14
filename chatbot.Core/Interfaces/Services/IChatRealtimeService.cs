using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;

namespace chatbot.Core.Interfaces.Services
{
    public interface IChatRealtimeService
    {
        Task MessageSentAsync(Guid conversationId,MessageDto message);

        Task GroupUpdatedAsync( Guid conversationId,ConversationDetailsDto group);

        Task MemberAddedAsync( Guid conversationId,ConversationMemberDto member);

        Task MemberRemovedAsync( Guid conversationId, Guid userId);

        Task MemberRoleChangedAsync(Guid conversationId, Guid userId, ConversationRole role);

        Task MemberMutedAsync(Guid conversationId,Guid userId, DateTime? until);

        Task MemberUnmutedAsync(Guid conversationId,Guid userId);

        Task MemberBannedAsync(Guid conversationId, Guid userId);

        Task MemberUnbannedAsync(Guid conversationId, Guid userId);

        Task OwnershipTransferredAsync(Guid conversationId,Guid oldOwnerId, Guid newOwnerId);
        Task RemoveUserFromConversationAsync(Guid conversationId, Guid userId);
    }
}
