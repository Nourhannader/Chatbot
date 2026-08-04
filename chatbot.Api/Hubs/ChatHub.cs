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
        
    }
}
