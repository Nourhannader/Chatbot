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

namespace chatbot.Ef.Services
{
    public class NotificationService(IUnitOfWork unitOfWork,IPushNotificationFactory pushFactory
        ,IRealtimeNotificationService realtimeService,INotificationDecisionService decisionService,ILogger logger) : INotificationService
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
            var decision = await decisionService.GetOptionsAsync(userId, type, options.ConversationId, options.MessageId);

            var saveToDatabase = decision.SaveToDatabase && options.SaveToDatabase;

            var sendSignalR = decision.SendRealtimeNotification && options.SendRealtimeNotification;

            var sendPush = decision.SendPush && options.SendPush;

            Notification? notification = null;

            if (saveToDatabase)
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Body = body,
                    ReferenceId = options.MessageId?.ToString(),
                    Data = options.Data is not null
                        ? JsonSerializer.Serialize(options.Data)
                        : null,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await unitOfWork.Notifications.AddAsync(notification);

                await unitOfWork.SaveChangesAsync();
            }


            var dto = new NotificationDto
            {
                Id = notification?.Id ?? Guid.NewGuid(),
                Type = type,
                Title = title,
                Body = body,
                ReferenceId = options.MessageId?.ToString(),
                Data = options.Data is not null
                ? JsonSerializer.Serialize(options.Data)
                : null,
                IsRead = false,
                CreatedAt = notification?.CreatedAt ?? DateTime.UtcNow
            };

            if (sendSignalR)
            {
                await realtimeService.SendAsync(userId, dto);
            }

            if (!sendPush)
                return;

            var devices = await unitOfWork.UserDevices.GetActiveDevicesAsync(userId);

            foreach(var device in devices)
            {
                try
                {
                    var pushService =  pushFactory.Get(device.Provider);

                    await pushService.SendAsync( device.PushToken,title,body,options.Data);
                }
                catch
                {
                    // Add ILogger later.
                    // Failure on one device should not
                    // prevent the remaining devices.
                    logger.Log(LogLevel.Information, "Failure on one device should not prevent the remaining devices. ");
                }
            }

        }
    }
}
