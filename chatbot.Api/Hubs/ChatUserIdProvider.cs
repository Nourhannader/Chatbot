using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace chatbot.Api.Hubs
{
    public sealed class ChatUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
