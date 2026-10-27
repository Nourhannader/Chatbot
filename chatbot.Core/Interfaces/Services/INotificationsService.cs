using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendAsync( Guid userId,NotificationType type,string title, string body,NotificationOptions? options = null);

        Task<PagedResultDto<NotificationDto>> GetAsync(Guid userId,int pageNumber,int pageSize);

        Task MarkAsReadAsync(Guid userId,Guid notificationId);

        Task MarkAllAsReadAsync(Guid userId);

        Task<int> GetUnreadCountAsync( Guid userId);
        Task NotifyNewMessageAsync(Guid messageId, Guid conversationId, Guid senderId, CancellationToken cancellationToken);
        Task CreateNewMessageNotificationsAsync(Guid messageId, Guid conversationId, Guid senderId, CancellationToken cancellationToken);
        Task CreateReactionNotificationAsync(Guid messageId, Guid userId, ReactionType reactionType, CancellationToken cancellationToken);
        Task CreateReplyNotificationAsync(Guid messageId, Guid replyToMessageId, Guid conversationId, Guid senderId, Guid originalMessageSenderId, CancellationToken cancellationToken);
        Task CreateMemberAddedNotificationAsync(Guid conversationId, Guid userId, Guid addedByUserId, CancellationToken cancellationToken);
        Task CreateMemberRemovedNotificationAsync(Guid conversationId, Guid userId, Guid value, MemberRemovalType removalType, CancellationToken cancellationToken);
        Task CreateRoleChangedNotificationAsync(Guid conversationId, Guid userId, Guid changedByUserId, ConversationRole oldRole, ConversationRole newRole, CancellationToken cancellationToken);
    }
}
