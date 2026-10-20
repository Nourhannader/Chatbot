using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface INotificationDecisionService
    {
        Task<NotificationOptions> GetOptionsAsync(Guid userId, NotificationType type, Guid? conversationId = null, Guid? messageId = null);
    }
}
