using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public sealed class NotificationPreferenceService(IUnitOfWork unitOfWork,IMapper mapper) : INotificationPreferenceService
    {
        public async Task<NotificationPreferencesDto> GetAsync(Guid userId)
        {
            var preference = await unitOfWork.NotificationPreference.GetByUserIdAsync(userId);

            if (preference is null)
            {
                preference =
                    new NotificationPreferences
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        NewMessages = true,
                        MessageReactions = true,
                        MessageReplies = true,
                        Mentions = true,
                        PushNotifications = true,
                        SoundEnabled = true
                    };

                await unitOfWork.NotificationPreference.AddAsync(preference);

                await unitOfWork.SaveChangesAsync();
            }

            return mapper.Map<NotificationPreferencesDto>(preference);

        }

        public async Task UpdateAsync(Guid userId, UpdateNotificationPreferencesDto dto)
        {
            var preference = await unitOfWork.NotificationPreference.GetByUserIdAsync(userId);

            if (preference is null)
            {
                preference =
                    new NotificationPreferences
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId
                    };

                await unitOfWork.NotificationPreference.AddAsync(preference);
            }

            preference.NewMessages = dto.NewMessages;
            preference.MessageReactions = dto.MessageReactions;
            preference.MessageReplies = dto.MessageReplies;
            preference.Mentions = dto.Mentions;
            preference.PushNotifications = dto.PushNotifications;
            preference.SoundEnabled = dto.SoundEnabled;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
