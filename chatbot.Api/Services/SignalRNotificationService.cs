using chatbot.Api.Hubs;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Services
{
    public class SignalRNotificationService(IHubContext<ChatHub> hubContext) : IRealtimeNotificationService
    {
        public async Task SendAsync(Guid userId, NotificationDto notification)
        {
            await hubContext.Clients
                .User(userId.ToString())
                .SendAsync("NotificationReceived", notification);
        }

        public async Task SendUnreadCountAsync(Guid userId, int unreadCount)
        {
            await hubContext.Clients
            .User(userId.ToString())
            .SendAsync( "NotificationUnreadCountChanged",unreadCount);
        }
    }
}
