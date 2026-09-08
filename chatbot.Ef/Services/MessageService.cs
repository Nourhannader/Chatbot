using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public class MessageService(IUnitOfWork unitOfWork,IStorageService storage,IFileValidationService validator) : IMessageService
    {
        public async Task DeleteForEveryoneAsync(Guid messageId, Guid userId)
        {
            var message =
           await unitOfWork.Messages
               .GetByIdAsync(messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            if (message.SenderId != userId)
                throw new UnauthorizedAccessException("Only sender can delete this message.");

            if (message.IsDeletedForEveryone)
                return;

            message.IsDeletedForEveryone = true;

            message.DeletedForEveryoneAt =
                DateTime.UtcNow;

            message.Content = "This message was deleted";

            await unitOfWork
                .SaveChangesAsync();

        }

        public async Task DeleteForMeAsync(Guid messageId, Guid userId)
        {
            var message =
            await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            var exists =
                await unitOfWork.Messages
                    .IsDeletedForUserAsync(
                        messageId,
                        userId);

            if (exists)
                return;

            var deletion = new MessageDeletion
            {
                MessageId = messageId,
                UserId = userId,
                DeletedAt = DateTime.UtcNow
            };

            await unitOfWork.Messages
                .AddDeletionAsync(deletion);

            await unitOfWork
                .SaveChangesAsync();
        }

        public async Task<PagedResultDto<Message>> GetMessagesAsyns(Guid conversationId, int page, int pageSize)
        {
            return await unitOfWork.Messages
                .GetConversationMessagesAsync(conversationId, page, pageSize);
        }

        public Task MarkDeliveredAsync(Guid messageId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task MarkReadAsync(Guid conversationId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Message> ReplyAsync(Guid senderId, ReplyMessageDto dto)
        {
            var originalMessage = await unitOfWork.Messages.GetByIdAsync(dto.MessageId);
            if (originalMessage == null)
                throw new Exception("Message not found.");
            if (originalMessage.ConversationId != dto.ConversationId)
                throw new InvalidOperationException("The original message does not belong to the this conversation.");

            var message = new Message
            {
                SenderId = senderId,
                ConversationId = dto.ConversationId,
                Content = dto.Content,
                MessageType = dto.Type,
                SendAt = DateTime.UtcNow,
                ReplyToMessageId = dto.MessageId

            };
           await unitOfWork.Messages.AddAsync(message);
            await unitOfWork.SaveChangesAsync();
            return message;
        }

        public Task<MessageDto> SendFileAsync(SendFileDto filedto)
        {
           // validator.ValidateFile(filedto.)
           throw new NotImplementedException();
        }

        public async Task SendMessageAsync(
           Guid conversationId,
           Guid senderId,
          string? content,
          IEnumerable<IFormFile>? files,
          CancellationToken cancellationToken = default)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                SendAt = DateTime.UtcNow
            };

            
            await unitOfWork.Messages.AddAsync(message);

            if (files != null && files.Any())
            {
                await storage.UploadManyAsync(
                    files,
                    message.Id,
                    "messages",
                    senderId,
                    cancellationToken);
            }


            await unitOfWork.SaveChangesAsync();
        }

    }
}
