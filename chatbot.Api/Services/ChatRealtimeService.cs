using chatbot.Api.Hubs;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Manager;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Services
{
    public sealed class ChatRealtimeService : IChatRealtimeService
    {
        private readonly IHubContext<ChatHub, IChatClient> hubContext;
        private readonly IUserConnectionManager connectionManager;

        public ChatRealtimeService(IHubContext<ChatHub, IChatClient> hubContext, IUserConnectionManager connectionManager)
        {
            this.hubContext = hubContext;
            this.connectionManager = connectionManager;
        }

        public Task MediaProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task MessageDeletedForUserAsync(Guid userId, MessageDeletedForUserDto dto)
        {
            throw new NotImplementedException();
        }

        public Task SendToConversationAsync(Guid conversationId, string method, object payload, CancellationToken cancellationToken = default)
        {
            return hubContext.Clients
                .Group($"conversation:{conversationId}")
                .SendAsync(method, payload, cancellationToken);
        }

        public Task SendToUserAsync(Guid userId, string method, object payload, CancellationToken cancellationToken = default)
        {
            return hubContext.Clients
                .User(userId.ToString())
                .SendAsync(method, payload, cancellationToken);
        }

        public Task VoiceNoteProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class ChatHub : Hub<IChatClient>
    {
        private const string ConversationGroupPrefix = "conversation:";

        public override Task OnConnectedAsync()
        {
            // Implementation here
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Implementation here
            return base.OnDisconnectedAsync(exception);
        }

        public Task JoinConversation(Guid conversationId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetConversationGroup(conversationId));
        }

        public Task LeaveConversation(Guid conversationId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetConversationGroup(conversationId));
        }

        private Guid GetCurrentUserId()
        {
            // Implementation to retrieve current user ID
            throw new NotImplementedException();
        }

        private static string GetConversationGroup(Guid conversationId)
        {
            return $"{ConversationGroupPrefix}{conversationId}";
        }
    }
}
