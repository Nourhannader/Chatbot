using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using MimeKit;
using static chatbot.Core.Interfaces.Services.IChatRealtimeService;
using IChatRealtimeService = chatbot.Core.Interfaces.Services.IChatRealtimeService.IChatRealtimeService;

namespace chatbot.Ef.Services
{
    public class MessageStatusService(IUnitOfWork unitOfWork,IChatRealtimeService chatRealtimeService,IMapper mapper) : IMessageStatusService
    {
        public async Task CreateRecipientsAsync(Message message)
        {
            ArgumentNullException.ThrowIfNull(message);
            var members = await unitOfWork.ConversationMember.GetActiveMembersAsync(message.ConversationId);

            var recipients = members
                .Where(x => x.UserId != message.SenderId)
                .Select(x => new MessageRecipientStatus
                {
                    MessageId = message.Id,
                    RecipientId = x.UserId,
                    Status = MessageStatus.Sent,
                    SentAt = message.SendAt
                }).ToList();

            if (recipients.Count == 0)
                return;

            await unitOfWork.MessageStatuses.AddRangeAsync(recipients);
        }

        public async Task<List<MessageRecipientStatusDto>> GetReceiptsAsync(Guid messageId, Guid currentUserId)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(messageId);
            if (message == null)
                throw new NotFoundException("Message not found.");

            var isMember = await unitOfWork.ConversationMember
                    .IsActiveMemberAsync(message.ConversationId, currentUserId);

            if (!isMember)
            {
                throw new ForbiddenException("You are not a member of this conversation.");
            }

            var recipients = await unitOfWork.MessageStatuses
            .GetByMessageIdAsync(messageId);

            return mapper.Map<List<MessageRecipientStatusDto>>(recipients);

        }

        public async Task<MessageStatusSummaryDto> GetSummaryAsync(Guid messageId, Guid currentUserId)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
            {
                throw new NotFoundException("Message not found.");
            }


            var isMember = await unitOfWork.ConversationMember
                .IsActiveMemberAsync(message.ConversationId, currentUserId);

            if (!isMember)
            {
                throw new ForbiddenException("You are not a member of this conversation.");
            }
            var summary = await unitOfWork.MessageStatuses
                .GetSummaryAsync(messageId);

            return summary ?? new MessageStatusSummaryDto
            {
                MessageId = messageId,
                RecipientCount = 0,
                SentCount=0,
                DeliveredCount = 0,
                ReadCount = 0
            };
        }

        public async Task<bool> HasUserReadMessageAsync(Guid messageId, Guid recipientId)
        {
            var status = await unitOfWork.MessageStatuses
            .GetAsync(messageId, recipientId);

            return status?.ReadAt is not null;
        }

        public async Task MarkAsDeliveredAsync(Guid userId, Guid messageId)
        {
            var recipient = await unitOfWork.MessageStatuses.GetAsync(messageId, userId);
            if (recipient == null)
            {
                throw new NotFoundException("Message recipient status not found.");
            }
            if (recipient.DeliveredAt.HasValue)
                return;

            var now = DateTime.UtcNow;

            recipient.DeliveredAt = now;

            recipient.Status = MessageStatus.Delivered;

            var userName = recipient.Recipient?.UserName;


            await unitOfWork.SaveChangesAsync();

            var dto = new MessageRecipientStatusDto
            {
                MessageId = recipient.MessageId,
                UserId = recipient.RecipientId,
                UserName=userName,
                Status = MessageStatus.Delivered,
                SentAt = recipient.SentAt,
                DeliveredAt = recipient.DeliveredAt,
                ReadAt = recipient.ReadAt
            };

            await chatRealtimeService
                .MessageDeliveredAsync(recipient.Message.ConversationId, dto);
        }

        public async Task MarkAsReadAsync(Guid userId, Guid conversationId, Guid lastMessageId)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(lastMessageId);

            if (message == null)
                throw new NotFoundException("Message not found.");

            if (message.ConversationId != conversationId)
                throw new BadRequestException("Message does not belong to this conversation.");

            var isMember = await unitOfWork.ConversationMember
            .IsActiveMemberAsync(conversationId, userId);

            if (!isMember)
                throw new ForbiddenException("You are not a member of this conversation.");

            var now = DateTime.UtcNow;

            var recipients = await unitOfWork.MessageStatuses
                    .GetUnreadAsync(userId, conversationId, message.SendAt);

            if (recipients.Count == 0)
                return;

            foreach (var recipient in recipients)
            {
                recipient.DeliveredAt ??= now;
                recipient.ReadAt = now;
                recipient.Status =MessageStatus.Read;
            }

            await unitOfWork.SaveChangesAsync();

            await chatRealtimeService.MessagesReadAsync(conversationId,
                    new MessagesReadEventDto
                    {
                        UserId = userId,
                        ConversationId = conversationId,
                        LastMessageId = lastMessageId,
                        ReadAt = now
                    });

        }
    }
}
