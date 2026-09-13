using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class NotificationPreferenceService(IUnitOfWork unitOfWork) : INotificationPreferenceService
    {
        public async Task<NotificationPreferences> GetAsync(Guid userId)
        {
            var preference =
            await unitOfWork.NotificationPreference.GetByIdAsync(userId);

            if (preference != null)
                return preference;

            preference = new NotificationPreferences
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };

            await unitOfWork.NotificationPreference.AddAsync(preference);

            await unitOfWork.SaveChangesAsync();

            return preference;
        }

        public async Task UpdateAsync(Guid userId, UpdateNotificationPreferencesDto dto)
        {
            var preference =
             await unitOfWork.NotificationPreference.GetByIdAsync(userId);

            if (preference == null)
            {
                preference =
                    new NotificationPreferences
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId
                    };

                await unitOfWork.NotificationPreference.AddAsync(preference);
            }

            preference.NewMessages =
                dto.NewMessages;

            preference.MessageReactions =
                dto.MessageReactions;

            preference.Mentions =
                dto.Mentions;

            preference.PushNotifications =
                dto.PushNotifications;

            preference.SoundEnabled =
                dto.SoundEnabled;

            await unitOfWork.SaveChangesAsync();
        }
    }
}
