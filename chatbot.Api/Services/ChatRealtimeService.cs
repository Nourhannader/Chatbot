using chatbot.Api.Hubs;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Manager;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Services
{
    public sealed class ChatRealtimeService: IChatRealtimeService
    {
        private readonly IHubContext<ChatHub, IChatClient> hubContext;
        private readonly IUserConnectionManager connectionManager;
        public ChatRealtimeService(IHubContext<ChatHub,IChatClient> hubContext,IUserConnectionManager connectionManager)
        {
            this.hubContext=hubContext;
            this.connectionManager=connectionManager;
        }

        // ==========================================
        // Conversation
        // ==========================================

        public Task ConversationAddedAsync(
            Guid userId,
            ConversationDetailsDto conversation)
        {
            return hubContext.Clients
                .User(userId.ToString())
                .ConversationAdded(conversation);
        }

        public Task ConversationUpdatedAsync(
            Guid conversationId,
            ConversationDetailsDto conversation)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .ConversationUpdated(conversation);
        }

        public Task ConversationDeletedAsync(
            Guid conversationId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .ConversationDeleted(
                    conversationId);
        }

        public Task ConversationImageUpdatedAsync(
            Guid conversationId,
            ConversationImageDto image)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .ConversationImageUpdated(
                    conversationId,
                    image);
        }

        public Task ConversationImageRemovedAsync(
            Guid conversationId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .ConversationImageRemoved(
                    conversationId);
        }


        // ==========================================
        // Members
        // ==========================================

        public Task MemberAddedAsync(
            Guid conversationId,
            ConversationMemberDto member)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MemberAdded(member);
        }

        public Task MemberRemovedAsync(
            Guid conversationId,
            Guid userId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MemberRemoved(userId);
        }

        public Task MemberRoleChangedAsync(
            Guid conversationId,
            Guid userId,
            ConversationRole role)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MemberRoleChanged(
                    userId,
                    role);
        }

        public Task MemberMutedAsync(
            Guid conversationId,
            Guid userId,
            DateTime? until)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MemberMuted(
                    userId,
                    until);
        }

        public Task MemberUnmutedAsync(
            Guid conversationId,
            Guid userId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MemberUnmuted(userId);
        }

        public Task MemberBannedAsync(
            Guid conversationId,
            Guid userId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MemberBanned(userId);
        }

        public Task MemberUnbannedAsync(
            Guid conversationId,
            Guid userId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MemberUnbanned(userId);
        }

        public Task OwnershipTransferredAsync(
            Guid conversationId,
            Guid oldOwnerId,
            Guid newOwnerId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .OwnershipTransferred(
                    oldOwnerId,
                    newOwnerId);
        }


        // ==========================================
        // Messages
        // ==========================================

        public Task MessageSentAsync(
            Guid conversationId,
            MessageDto message)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .ReceiveMessage(message);
        }

        public Task MessageEditedAsync(
            Guid conversationId,
            MessageDto message)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MessageEdited(message);
        }

        public Task MessageDeletedAsync(
            Guid conversationId,
            Guid messageId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MessageDeleted(messageId);
        }

        public Task MessageReactionAddedAsync(
            Guid conversationId,
            Guid messageId,
            MessageReactionDto reaction)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MessageReactionAdded(
                    messageId,
                    reaction);
        }

        public Task MessageReactionRemovedAsync(
            Guid conversationId,
            Guid messageId,
            Guid userId)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name(
                    conversationId))
                .MessageReactionRemoved(
                    messageId,
                    userId);
        }


        // ==========================================
        // Read
        // ==========================================

        public Task ConversationReadAsync(
            Guid conversationId,
            Guid userId,
            DateTime readAt)
        {
            return hubContext.Clients
                .Group(ConversationGroup.Name( conversationId))
                .ConversationRead(conversationId, userId,readAt);
        }


        // ==========================================
        // Force remove user
        // ==========================================

        public async Task RemoveUserFromConversationAsync( Guid conversationId, Guid userId)
        {
            var connections =
                await connectionManager.GetConnectionsAsync(userId);

            var groupName = ConversationGroup.Name(conversationId);

            foreach (var connectionId in connections)
            {
                await hubContext.Groups.RemoveFromGroupAsync(connectionId,groupName);
            }

            await hubContext.Clients.User(userId.ToString()).RemovedFromConversation( conversationId);
        }
    }
}
