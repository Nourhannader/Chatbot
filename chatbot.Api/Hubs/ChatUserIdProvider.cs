using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace chatbot.Api.Hubs
{
    public sealed class ChatUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? connection.User?.FindFirstValue("sub");

            return Guid.TryParse(userId, out var id) ? id.ToString() : null;
        }
    }
}
