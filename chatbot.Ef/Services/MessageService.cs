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
    public class MessageService(IUnitOfWork unitOfWork) : IMessageService
    {
        public async Task DeleteForEveryoneAsync(string messageId, string userId)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(messageId);
            if (message == null)
            {
                throw new Exception("Message not found");
            }
            if (message.SenderId != userId)
            {
                throw new Exception("Unauthorized");
            }
            message.IsDeletedForEveryone = true;
            message.DeletedAt = DateTime.UtcNow;

            unitOfWork.Messages.Update(message);
            await unitOfWork.SaveChangesAsync();

        }

        public async Task<PagedResultDto<Message>> GetMessagesAsyns(string conversationId, int page, int pageSize)
        {
            return await unitOfWork.Messages
                .GetConversationMessagesAsync(conversationId, page, pageSize);
        }

        public Task MarkDeliveredAsync(string messageId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task MarkReadAsync(string conversationId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Message> SendMessageAsync(string senderId, SendMessageDto messageDto)
        {
            var Message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = messageDto.ConversationId,
                Content = messageDto.Content,
                SenderId = senderId,
                Type = messageDto.Type,
                FileUrl = messageDto.FileUrl,
                FileName = messageDto.FileName,
                FileSizeBytes = messageDto.FileSizeBytes,
                FileDurationSeconds = messageDto.FileDurationSeconds,
                SentAt = DateTime.UtcNow

            };
            await unitOfWork.Messages.AddAsync(Message);
            
            await unitOfWork.SaveChangesAsync();
            return Message;
        }
    }
}
