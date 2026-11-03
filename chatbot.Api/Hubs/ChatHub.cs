using System.Security.Claims;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Manager;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Hubs
{
    [Authorize]
    [Authorize]
    public sealed class ChatHub(
    IUnitOfWork unitOfWork,
    ILogger<ChatHub> logger)
    : Hub
    {
        private const string ConversationGroupPrefix = "conversation:";

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();

            var connection = new UserConnection
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ConnectionId = Context.ConnectionId,
                IsOnline = true,
                ConnectedAt = DateTime.UtcNow
            };

            await unitOfWork.UserConnections.AddAsync(connection);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation(
                "User {UserId} connected. ConnectionId: {ConnectionId}",
                userId,
                Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = await unitOfWork.UserConnections
                .GetByConnectionIdAsync(Context.ConnectionId);

            if (connection is not null)
            {
                connection.IsOnline = false;
                connection.DisconnectedAt = DateTime.UtcNow;

                await unitOfWork.SaveChangesAsync();
            }

            if (exception is not null)
            {
                logger.LogWarning(
                    exception,
                    "Connection {ConnectionId} disconnected unexpectedly.",
                    Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinConversation(Guid conversationId)
        {
            var userId = GetCurrentUserId();

            var isMember = await unitOfWork.ConversationMember
                .ExistsAsync(conversationId, userId);

            if (!isMember)
                throw new HubException("You are not a member of this conversation.");

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GetConversationGroup(conversationId));

            logger.LogInformation(
                "User {UserId} joined conversation {ConversationId}",
                userId,
                conversationId);
        }

        public async Task LeaveConversation(Guid conversationId)
        {
            var userId = GetCurrentUserId();

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GetConversationGroup(conversationId));

            logger.LogInformation(
                "User {UserId} left conversation {ConversationId}",
                userId,
                conversationId);
        }

        private Guid GetCurrentUserId()
        {
            var userId = Context.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new HubException("Invalid user identity.");

            return parsedUserId;
        }

        private static string GetConversationGroup(Guid conversationId)
        {
            return $"{ConversationGroupPrefix}{conversationId}";
        }
    }
}
