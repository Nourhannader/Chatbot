using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class ForwardService(IUnitOfWork unitOfWork) : IForwardService
    {
        public async Task<List<Message>> ForwardAsync(Guid senderId, ForwardMessageDto dto)
        {
            var original = await unitOfWork.Messages.GetByIdAsync(dto.MessageId);
            if(original ==null)
                throw new KeyNotFoundException("Message not found.");
            var messages=new List<Message>();
            foreach(var id in dto.ConversationIds)
            {
                if (!await unitOfWork.ForwardMessages.ConversationExistsAsync(Guid.Parse(id)))
                    continue;
                if (!await unitOfWork.ForwardMessages.IsMemberAsync(Guid.Parse(id), senderId))
                    continue;

                messages.Add(new Message
                {
                    SenderId = senderId,
                    ConversationId = Guid.Parse(id),
                    MessageType = original.MessageType,
                    Content = original.Content,
                    OriginalMessageId = original.Id,
                    IsForwarded = true,
                    SendAt=DateTime.UtcNow
                });

            }
            await unitOfWork.ForwardMessages.AddRangeAsync(messages);
            await unitOfWork.SaveChangesAsync();

            return messages;

        }
    }
}
