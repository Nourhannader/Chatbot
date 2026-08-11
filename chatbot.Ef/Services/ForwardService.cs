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
        public async Task<List<Message>> ForwardAsync(string senderId, ForwardMessageDto dto)
        {
            var original = await unitOfWork.Messages.GetByIdAsync(dto.MessageId);
            if(original ==null)
                throw new Exception("Message not found.");
            var messages=new List<Message>();
            foreach(var id in dto.ConversationIds)
            {
                if (!await unitOfWork.ForwardMessages.ConversationExistsAsync(id))
                    continue;
                if (!await unitOfWork.ForwardMessages.IsMemberAsync(id, senderId))
                    continue;

                messages.Add(new Message
                {
                    SenderId = senderId,
                    ConversationId = id,
                    Type = original.Type,
                    Content = original.Content,
                    FileUrl = original.FileUrl,
                    FileName = original.FileName,
                    FileSizeBytes = original.FileSizeBytes,
                    FileDurationSeconds = original.FileDurationSeconds,
                    OriginalMessageId = original.Id,
                    IsForwarded = true,
                    SentAt=DateTime.UtcNow
                });

            }
            await unitOfWork.ForwardMessages.AddRangeAsync(messages);
            await unitOfWork.SaveChangesAsync();

            return messages;

        }
    }
}
