using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.MessageRead;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class MessageReadService(IUnitOfWork unitOfWork,IMapper mapper) : IMessageReadService
    {
        public async Task<IEnumerable<MessageReadDto>> GetMessageReadersAsync(Guid messageId)
        {
            var reads=await unitOfWork.MessageReads.GetByMessageIdAsync(messageId);

            return mapper.Map<IEnumerable<MessageReadDto>>(reads);
        }

        public async Task<bool> HasUserReadMessageAsync(Guid messageId, Guid userId)
        {
            return await unitOfWork.MessageReads.ExistsAsync(messageId, userId);
        }

        public async Task<MessageReadDto> MarkAsReadAsync(Guid messageId, Guid userId)
        {
            // 1. Validate input
            if (messageId == Guid.Empty)
            {
                throw new ArgumentException("Message ID is required.", nameof(messageId));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID is required.", nameof(userId));
            }

            // 2. Check whether the message exists
            var message = await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
            {
                throw new KeyNotFoundException("Message was not found.");
            }

            // 3. Check whether the user belongs to the conversation
            var isMember = await unitOfWork.ConversationMember
                .IsMemberAsync(message.ConversationId, userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this conversation.");
            }

            // 4. Check whether the message has already been marked as read
            var existingRead = await unitOfWork.MessageReads.GetByMessageAndUserAsync(messageId, userId);

            if (existingRead != null)
            {
                return mapper.Map<MessageReadDto>(existingRead);
            }

            // 5. Create read record
            var messageRead = new MessageRead
            {
                MessageId = messageId,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            };

            // 6. Add record
            await unitOfWork.MessageReads.AddAsync(messageRead);

            // 7. Save changes through UnitOfWork
            await unitOfWork.SaveChangesAsync();

            // 8. Return DTO
            return mapper.Map<MessageReadDto>(messageRead);
        }
    }
}
