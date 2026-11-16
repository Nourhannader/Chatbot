using System.Security.Claims;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.DTOs.Chat;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Manager;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Services;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Hubs
{
    [Authorize]
    [Authorize]
    public sealed class ChatHub(IAuthorizationService authorizationService, IPresenceService presenceService, ITypingService typingService,
    IUnitOfWork unitOfWork, ILogger<ChatHub> logger) : Hub<IChatClient>
    {
        // -------------------------------------------------
        // Connection
        // -------------------------------------------------
        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = CurrentUserId;
                var sessionId = CurrentDeviceSessionId;

                // Validate the DeviceSession associated with this JWT.
                var session = await unitOfWork.Sessions.GetActiveSessionAsync(sessionId, userId);

                if (session is null)
                {
                    Context.Abort();
                    return;
                }

                var connection = new UserConnection
                {
                    UserId = userId,
                    DeviceSessionId = sessionId,
                    ConnectionId = Context.ConnectionId,
                    IsOnline = true,
                    CreatedAt = DateTime.UtcNow,
                    ConnectedAt = DateTime.UtcNow
                };

                await unitOfWork.UserConnections.AddAsync(connection);
                await unitOfWork.SaveChangesAsync();

                // Each user receives events on all their connections.
                await Groups.AddToGroupAsync( Context.ConnectionId,ChatHubConstants.GetUserGroup(userId));

                await presenceService.UserConnectedAsync(userId);

                logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error connecting SignalR client {ConnectionId}",Context.ConnectionId);

                Context.Abort();
            }
        }

        // -------------------------------------------------
        // Disconnect
        // -------------------------------------------------
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            try
            {
                // Remove this connection from all typing states.
                var stoppedTyping = await typingService.RemoveConnectionAsync(connectionId);

                foreach (var item in stoppedTyping)
                {
                    await Clients
                        .Group(ChatHubConstants.GetConversationGroup(item.ConversationId))
                        .UserStoppedTyping(item.ConversationId, item.UserId);
                }

                // Get the active connection before marking it disconnected.
                var connection = await unitOfWork.UserConnections.GetByConnectionIdAsync(connectionId);

                if (connection is not null)
                {
                    var userId = connection.UserId;

                    await unitOfWork.UserConnections.RemoveAsync(connectionId);

                    await unitOfWork.SaveChangesAsync();

                    // This checks whether the user has other active connections.
                    await presenceService.UserDisconnectedAsync(userId);

                    logger.LogInformation("User {UserId} disconnected from {ConnectionId}", userId,connectionId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error disconnecting SignalR client {ConnectionId}", connectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        // -------------------------------------------------
        // Join Conversation
        // -------------------------------------------------
        public async Task JoinConversation(Guid conversationId)
        {
            if (conversationId == Guid.Empty)
                throw new HubException("Invalid conversation id.");

            await AuthorizeConversationAsync( conversationId, ConversationPermissions.View);

            var groupName =ChatHubConstants.GetConversationGroup(conversationId);

            await Groups.AddToGroupAsync(Context.ConnectionId,groupName);

            logger.LogInformation("User {UserId} joined conversation {ConversationId}", CurrentUserId, conversationId);
        }

        // -------------------------------------------------
        // Leave Conversation
        // -------------------------------------------------

        public async Task LeaveConversation(Guid conversationId)
        {
            if (conversationId == Guid.Empty)
                throw new HubException("Invalid conversation id.");

            // Stop this connection's typing state first.
            var stillTyping = await typingService.StopTypingAsync(conversationId,CurrentUserId, Context.ConnectionId);

            if (!stillTyping)
            {
                await Clients
                    .Group(ChatHubConstants.GetConversationGroup(conversationId))
                    .UserStoppedTyping(conversationId, CurrentUserId);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatHubConstants.GetConversationGroup(conversationId));

            logger.LogInformation("User {UserId} left conversation {ConversationId}",CurrentUserId,conversationId);
        }

        // -------------------------------------------------
        // Start Typing
        // -------------------------------------------------

        public async Task StartTyping(Guid conversationId)
        {
            if (conversationId == Guid.Empty)
                throw new HubException("Invalid conversation id.");

            await AuthorizeConversationAsync(conversationId, ConversationPermissions.View);

            var userId = CurrentUserId;

            await typingService.StartTypingAsync( conversationId, userId, Context.ConnectionId);

            await Clients
                .OthersInGroup(ChatHubConstants.GetConversationGroup(conversationId))
                .UserTyping(new TypingUserDto
                {
                    ConversationId = conversationId,
                    UserId = userId
                });
        }

        // -------------------------------------------------
        // Stop Typing
        // -------------------------------------------------

        public async Task StopTyping(Guid conversationId)
        {
            if (conversationId == Guid.Empty)
                throw new HubException("Invalid conversation id.");

            var userId = CurrentUserId;

            var stillTyping = await typingService.StopTypingAsync(conversationId, userId, Context.ConnectionId);

            if (!stillTyping)
            {
                await Clients
                    .Group(ChatHubConstants.GetConversationGroup(conversationId))
                    .UserStoppedTyping(conversationId, userId);
            }
        }

        // -------------------------------------------------
        // Get Typing Users
        // -------------------------------------------------

        public async Task<List<Guid>> GetTypingUsers(Guid conversationId)
        {
            await AuthorizeConversationAsync(conversationId, ConversationPermissions.View);

            return await typingService.GetTypingUsersAsync(conversationId);
        }

        // -------------------------------------------------
        // Current User
        // -------------------------------------------------
        private Guid CurrentUserId
        {
            get
            {
                var userId =Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.User?.FindFirstValue("sub");

                if (!Guid.TryParse(userId, out var parsedUserId))
                    throw new HubException("Invalid user identity.");

                return parsedUserId;
            }
        }
        
        private Guid CurrentDeviceSessionId
        {
            get
            {
                var sessionId =Context.User?.FindFirstValue(ChatHubConstants.DeviceSessionIdClaim);

                if (!Guid.TryParse(sessionId, out var parsedSessionId))
                    throw new HubException("Invalid device session.");

                return parsedSessionId;
            }
        }

        // -------------------------------------------------
        // Authorization
        // -------------------------------------------------

        private async Task AuthorizeConversationAsync(Guid conversationId,string permission)
        {
            var resource = new ConversationResource(conversationId, null, null);

            var result = await authorizationService.AuthorizeAsync( Context.User!, resource, permission);

            if (!result.Succeeded)
            {
                throw new HubException("You are not authorized to access this conversation.");
            }
        }
    }
}
