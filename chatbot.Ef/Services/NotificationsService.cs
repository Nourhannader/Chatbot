using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Ef.Services
{
    public class NotificationService(IUnitOfWork unitOfWork,IPushNotificationFactory pushFactory
        ,IRealtimeNotificationService realtimeService)
    : INotificationService
    {
        public Task<List<NotificationDto>> GetAsync(Guid userId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadCountAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAllAsReadAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(Guid userId, NotificationType type, string title, string body, NotificationOptions? options = null)
        {
            options ??= new NotificationOptions();
            var preference=await unitOfWork.NotificationPreference.GetByIdAsync(userId);
            if (preference !=null)
            {
                if (type == NotificationType.NewMessage &&
                     !preference.NewMessages)
                {
                    return;
                }

                if (type == NotificationType.MessageReaction &&
                    !preference.MessageReactions)
                {
                    return;
                }

                if (type == NotificationType.MessageReply &&
                   !preference.MessageReplies)
                {
                    return;
                }

                if (type == NotificationType.Mention &&
                    !preference.Mentions)
                {
                    return;
                }
            }
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Body = body,
                Type = type,
                IsRead = false,
                Data = options.Data != null ? JsonSerializer.Serialize(options.Data) : null,
                CreatedAt = DateTime.UtcNow

            };
            //save notification
            if (options.SaveToDatabase)
            {
                await unitOfWork.Notifications.AddAsync(notification);
                await unitOfWork.SaveChangesAsync();
            }
            var dto = new NotificationDto
            {
                Id = notification.Id,

                Type = notification.Type,

                Title = notification.Title,

                Body = notification.Body,

                Data = notification.Data,

                IsRead = notification.IsRead,

                CreatedAt = notification.CreatedAt
            };
            //signalr
            if (options.SendSignalR)
            {
                await realtimeService.SendAsync(userId, dto);
            }
            //push
            if (options.SendPush && preference?.PushNotifications != false)
            {
                var devices = await unitOfWork.UserDevices.GetActiveDevicesAsync(userId);
                foreach (var device in devices)
                {
                    try
                    {
                        var pushService = pushFactory.Get(device.Provider);
                        await pushService.SendAsync(device.PushToken, title, body, options.Data);
                    }
                    catch(Exception ex) 
                    {
                        //log error


                    }
                }
            }
        }
    }
}
