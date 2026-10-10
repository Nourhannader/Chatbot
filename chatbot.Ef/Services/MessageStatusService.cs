using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class MessageStatusService(IUnitOfWork unitOfWork) : IMessageStatusService
    {
        private static MessageRecipientStatusDto MapToDto(MessageRecipientStatus entity)
        {
            return new MessageRecipientStatusDto
            {
                Id = entity.Id,
                MessageId = entity.MessageId,
                RecipientId = entity.RecipientId,
                RecipientName = entity.Recipient?.UserName,
                Status = entity.Status,
                SentAt = entity.SentAt,
                DeliveredAt = entity.DeliveredAt,
                ReadAt = entity.ReadAt
            };
        }
        public async Task<List<MessageRecipientStatusDto>> GetByMessageAsync(Guid messageId)
        {
            var statuses = await unitOfWork.MessageStatuses
            .GetByMessageAsync(messageId);

            return statuses.Select(MapToDto).ToList();
        }

        public async Task<List<MessageRecipientStatusDto>> GetReadersAsync(Guid messageId)
        {
            var readers = await unitOfWork.MessageStatuses
            .GetReadersAsync(messageId);

            return readers.Select(MapToDto).ToList();
        }

        public async Task<MessageStatus?> GetStatusAsync(Guid messageId, Guid recipientId)
        {
            return await unitOfWork.MessageStatuses.GetStatusAsync(messageId, recipientId);
        }

        public async Task<bool> HasUserReadMessageAsync(Guid messageId, Guid recipientId)
        {
            var status = await unitOfWork.MessageStatuses
            .GetAsync(messageId, recipientId);

            return status?.ReadAt is not null;
        }

        public async Task MarkAsDeliveredAsync(Guid messageId, Guid recipientId)
        {
            var now = DateTime.UtcNow;

            var status = await unitOfWork.MessageStatuses
                .GetAsync(messageId, recipientId);

            if (status is null)
            {
                status = new MessageRecipientStatus
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    RecipientId = recipientId,
                    Status = MessageStatus.Delivered,
                    SentAt = now,
                    DeliveredAt = now
                };

                await unitOfWork.MessageStatuses.AddAsync(status);
                await unitOfWork.SaveChangesAsync();
                return;
            }

            if (status.Status >= MessageStatus.Delivered)
                return;

            status.Status = MessageStatus.Delivered;
            status.SentAt ??= now;
            status.DeliveredAt ??= now;

            unitOfWork.MessageStatuses.Update(status);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid messageId, Guid recipientId)
        {
            var now = DateTime.UtcNow;

            var status = await unitOfWork.MessageStatuses
                .GetAsync(messageId, recipientId);

            if (status is null)
            {
                // A read event implies that the message was delivered.
                status = new MessageRecipientStatus
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    RecipientId = recipientId,
                    Status = MessageStatus.Read,
                    SentAt = now,
                    DeliveredAt = now,
                    ReadAt = now
                };

                await unitOfWork.MessageStatuses.AddAsync(status);
                await unitOfWork.SaveChangesAsync();
                return;
            }

            if (status.Status >= MessageStatus.Read)
                return;

            status.Status = MessageStatus.Read;
            status.SentAt ??= now;
            status.DeliveredAt ??= now;
            status.ReadAt ??= now;

            unitOfWork.MessageStatuses.Update(status);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsSentAsync(Guid messageId, Guid recipientId)
        {
            var status = await unitOfWork.MessageStatuses
            .GetAsync(messageId, recipientId);

            if (status is not null)
            {
                if (status.SentAt is null)
                {
                    status.SentAt = DateTime.UtcNow;
                    unitOfWork.MessageStatuses.Update(status);
                    await unitOfWork.SaveChangesAsync();
                }

                return;
            }

            status = new MessageRecipientStatus
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                RecipientId = recipientId,
                Status = MessageStatus.Sent,
                SentAt = DateTime.UtcNow
            };

            await unitOfWork.MessageStatuses.AddAsync(status);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
