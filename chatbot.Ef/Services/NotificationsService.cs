using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using chatbot.Ef.Outbox;
using chatbot.Core.Events.Notifications;

namespace chatbot.Ef.Services
{
    public class NotificationService(IUnitOfWork unitOfWork,INotificationDecisionService decisionService,ILogger logger,
        OutboxMessageFactory outboxMessageFactory,TimeProvider timeProvider) : INotificationService
    {
        public async Task<PagedResultDto<NotificationDto>> GetAsync(Guid userId, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = Math.Clamp(pageSize, 1, 100);

            var notifications = await unitOfWork.Notifications.GetUserNotificationsAsync(userId, pageNumber, pageSize);

            var totalCount = await unitOfWork.Notifications.GetUnreadCountAsync(userId);

            return new PagedResultDto<NotificationDto>
            {
                Items = notifications.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Type = n.Type,
                    Title = n.Title,
                    Body = n.Body,
                    ReferenceId = n.ReferenceId,
                    Data = n.Data,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                }).ToList(),
                TotalCount = totalCount
            };
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
           return await unitOfWork.Notifications.GetUnreadCountAsync(userId);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await unitOfWork.Notifications.MarkAllAsReadAsync(userId);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            await unitOfWork.Notifications.MarkAsReadAsync(userId, notificationId);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SendAsync(Guid userId, NotificationType type, string title, string body, NotificationOptions? options = null)
        {
            options ??= new NotificationOptions();
            var now = timeProvider.GetUtcNow().UtcDateTime;

            var decision = await decisionService.GetOptionsAsync(userId, type, options.ConversationId, options.MessageId);

            var saveToDatabase = decision.SaveToDatabase && options.SaveToDatabase;

            var sendRealtime = decision.SendRealtimeNotification && options.SendRealtimeNotification;

            var sendPush = decision.SendPush && options.SendPush;

            //nothing to do, return early
            if (!saveToDatabase && !sendRealtime && !sendPush)
                return;

            var notificationId = Guid.NewGuid();

            var serializedData = options.Data is not null
                ? JsonSerializer.Serialize(options.Data) : null;

            if (saveToDatabase)
            {
               var notification = new Notification
                {
                    Id = notificationId,
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Body = body,
                    ReferenceId = options.MessageId?.ToString(),
                    Data = serializedData,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await unitOfWork.Notifications.AddAsync(notification);

            }

            if (sendRealtime || sendPush)
            {
                var domainEvent = new NotificationCreatedEvent(notificationId, userId, type,
                    title, body, options.ConversationId, options.MessageId,serializedData, now,sendRealtime, sendPush,options.SoundEnabled);

                var outboxMessage =outboxMessageFactory.Create(domainEvent);

                await unitOfWork.OutboxMessages.AddAsync(outboxMessage);
            }

            await unitOfWork.SaveChangesAsync();
            logger.LogInformation(
            "Notification {NotificationId} created for user {UserId}. Database: {SaveToDatabase}, Realtime: {SendRealtime}, Push: {SendPush}",
            notificationId, userId,saveToDatabase, sendRealtime, sendPush);
        }
    }
}
