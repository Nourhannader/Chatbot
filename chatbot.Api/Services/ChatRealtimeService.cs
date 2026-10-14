using chatbot.Api.Hubs;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Manager;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Services
{
    public class ChatRealtimeService : IChatRealtimeService
    {
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IUserConnectionManager connectionManager;

        public ChatRealtimeService(
            IHubContext<ChatHub> hubContext,IUserConnectionManager connectionManager)
        {
            this.hubContext = hubContext;
            this.connectionManager = connectionManager;
        }

        private static string GroupName(Guid conversationId)
            => $"conversation:{conversationId}";

        public Task MessageSentAsync(Guid conversationId, MessageDto message)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("ReceiveMessage", message);
        }

        public Task GroupUpdatedAsync(Guid conversationId, ConversationDetailsDto group)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("GroupUpdated", group);
        }

        public Task MemberAddedAsync(Guid conversationId, ConversationMemberDto member)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("MemberAdded", member);
        }

        public Task MemberRemovedAsync(Guid conversationId, Guid userId)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("MemberRemoved", userId);
        }

        public Task MemberRoleChangedAsync(Guid conversationId, Guid userId, ConversationRole role)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("MemberRoleChanged", userId, role);
        }

        public Task MemberMutedAsync(Guid conversationId, Guid userId, DateTime? until)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("MemberMuted", userId, until);
        }

        public Task MemberUnmutedAsync(Guid conversationId, Guid userId)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("MemberUnmuted", userId);
        }

        public Task MemberBannedAsync(Guid conversationId, Guid userId)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("MemberBanned", userId);
        }

        public Task MemberUnbannedAsync(Guid conversationId, Guid userId)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("MemberUnbanned", userId);
        }

        public Task OwnershipTransferredAsync(Guid conversationId, Guid oldOwnerId, Guid newOwnerId)
        {
            return hubContext.Clients
                .Group(GroupName(conversationId))
                .SendAsync("OwnershipTransferred", oldOwnerId, newOwnerId);
        }

        public async Task RemoveUserFromConversationAsync(Guid conversationId,Guid userId)
        {
            var connections =
                await connectionManager
                    .GetConnectionsAsync(userId);

            var groupName =
                GroupName(conversationId);

            foreach (var connectionId in connections)
            {
                await hubContext.Groups
                    .RemoveFromGroupAsync(
                        connectionId,
                        groupName);
            }

            await hubContext.Clients
                .User(userId.ToString())
                .RemovedFromConversation(
                    conversationId);
        }
    }
}
