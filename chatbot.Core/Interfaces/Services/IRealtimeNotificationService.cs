using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface IRealtimeNotificationService
    {
        Task SendAsync(Guid userId, NotificationDto notification);
    }
}
