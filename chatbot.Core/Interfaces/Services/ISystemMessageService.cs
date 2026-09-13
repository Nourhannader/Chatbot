using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface ISystemMessageService
    {
        Task<Message> CreateAsync(Guid conversationId,SystemMessageType type,
        Guid? actorId = null,
        Guid? targetUserId = null);
    }
}
