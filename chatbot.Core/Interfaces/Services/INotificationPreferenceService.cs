using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface INotificationPreferenceService
    {
        Task<NotificationPreferencesDto> GetAsync(Guid userId);

        Task UpdateAsync(Guid userId,UpdateNotificationPreferencesDto dto);
    }
}
