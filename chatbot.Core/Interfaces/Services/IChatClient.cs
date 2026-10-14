using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;

namespace chatbot.Core.Interfaces.Services
{
    public interface IChatClient
    {
        Task ReceiveMessage(
            MessageDto message);

        Task GroupUpdated(
            ConversationDetailsDto group);

        Task MemberAdded(
            ConversationMemberDto member);

        Task MemberRemoved(
            Guid userId);

        Task MemberRoleChanged(
            Guid userId,
            ConversationRole role);

        Task MemberMuted(
            Guid userId,
            DateTime? until);

        Task MemberUnmuted(
            Guid userId);

        Task MemberBanned(
            Guid userId);

        Task MemberUnbanned(
            Guid userId);

        Task OwnershipTransferred(
            Guid oldOwnerId,
            Guid newOwnerId);

        Task RemovedFromConversation(
            Guid conversationId);

        Task GroupDeleted(
            Guid conversationId);
    }
}
