using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Manager;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Hubs
{
    [Authorize]
    public sealed class ChatHub(
    IConversationAuthorizationService authorizationService,
    IUserConnectionManager connectionManager)
    : Hub<IChatClient>
    {
        public override async Task OnConnectedAsync()
        {
            if (!Context.User!
                .TryGetUserId(out var userId))
            {
                Context.Abort();
                return;
            }

            await connectionManager
                .AddConnectionAsync(
                    userId,
                    Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            if (Context.User!
                .TryGetUserId(out var userId))
            {
                await connectionManager
                    .RemoveConnectionAsync(
                        userId,
                        Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(
                exception);
        }

        public async Task JoinConversation(
            Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(
                Context.User!,
                conversationId,
                ConversationPermissions.View);

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                ConversationGroup.Name(
                    conversationId));
        }

        public Task LeaveConversation(
            Guid conversationId)
        {
            return Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                ConversationGroup.Name(
                    conversationId));
        }

        public async Task TypingStarted(
            Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(
                Context.User!,
                conversationId,
                ConversationPermissions.SendMessage);

            var userId =
                Context.User!.GetUserId();

            await Clients
                .OthersInGroup(
                    ConversationGroup.Name(
                        conversationId))
                .UserTyping(
                    conversationId,
                    userId);
        }

        public async Task TypingStopped(
            Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(
                Context.User!,
                conversationId,
                ConversationPermissions.SendMessage);

            var userId =
                Context.User!.GetUserId();

            await Clients
                .OthersInGroup(
                    ConversationGroup.Name(
                        conversationId))
                .UserStoppedTyping(
                    conversationId,
                    userId);
        }
    }
}
