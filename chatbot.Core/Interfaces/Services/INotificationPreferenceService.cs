using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface INotificationPreferenceService
    {
        Task<NotificationPreferences> GetAsync(Guid userId);

        Task UpdateAsync(Guid userId,UpdateNotificationPreferencesDto dto);
    }
}
