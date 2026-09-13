using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Services.Helper;
using chatbot.Ef.UnitOfWork;

namespace chatbot.Ef.Services
{
    public class SystemMessageService : ISystemMessageService
    {
        private readonly SystemMessageHelper systemMessage;
        private readonly IUnitOfWork unitOfWork;
        public SystemMessageService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            systemMessage = new SystemMessageHelper(unitOfWork);
        }
        public async Task<Message> CreateAsync(Guid conversationId, SystemMessageType type, Guid? actorId = null, Guid? targetUserId = null)
        {
            var content = await systemMessage.BuildContentAsync(type, actorId,targetUserId);

            var message = new Message
            {
                Id = Guid.NewGuid(),

                ConversationId = conversationId,

                SenderId = actorId ?? Guid.Empty,

                Content = content,

                IsSystemMessage = true,

                SystemMessageType = type,

                SendAt = DateTime.UtcNow
            };

            await unitOfWork.Messages.AddAsync( message);

            await unitOfWork.SaveChangesAsync();

            return message;
        }
    }
    
}
