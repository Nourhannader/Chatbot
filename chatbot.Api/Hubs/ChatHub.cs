using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }
        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            await _chatService.SetUserOnline(Context.UserIdentifier, Context.ConnectionId);

        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _chatService.SetUserOffline(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int chatId,string message)
        {
            var senderId=Context.UserIdentifier;
            if (!await _chatService.IsMember(chatId, senderId))
            {
                return;
            }
            if (await _chatService.IsBlocked(chatId, senderId))
                return;

            var msg = await _chatService.CreateMessage(chatId, senderId, message);

            
            await Clients.Group(chatId.ToString())
                .SendAsync("ReceiveMessage", msg);

        }
    }
}
