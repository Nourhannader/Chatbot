using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class MessageStatusService(IUnitOfWork unitOfWork) : IMessageStatusService
    {
        public async Task<MessageStatus?> GetStatusAsync(Guid messageId, Guid recipientId)
        {
            return await unitOfWork.MessageStatuses.GetStatusAsync(messageId,recipientId) ;

        }

        public async Task MarkASSentAsync(Guid messageId, Guid recipientId)
        {
            var status = await unitOfWork.MessageStatuses.GetAsync(messageId, recipientId);
            if (status != null)
                return;
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

        public async Task MarkAsDeliveredAsync(Guid messageId, Guid recipientId)
        {
            var status = await unitOfWork.MessageStatuses.GetAsync(messageId, recipientId);
            if (status == null)
            {
                status = new MessageRecipientStatus
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    RecipientId = recipientId,
                    Status = MessageStatus.Delivered,
                    DeliveredAt = DateTime.UtcNow
                };
                await unitOfWork.MessageStatuses.AddAsync(status);
            }
            else if(status.Status < MessageStatus.Delivered)
            {
              
                status.Status = MessageStatus.Delivered;
                status.DeliveredAt = DateTime.UtcNow;
                unitOfWork.MessageStatuses.Update(status);
               
            }
          await  unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid messageId, Guid recipientId)
        {
            var status = await unitOfWork.MessageStatuses.GetAsync(messageId, recipientId);
            if (status == null)
            {
                status = new MessageRecipientStatus
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    RecipientId = recipientId,
                    Status = MessageStatus.Delivered,
                    DeliveredAt = DateTime.UtcNow
                };
                await unitOfWork.MessageStatuses.AddAsync(status);
            }
            else if (status.Status < MessageStatus.Seen)
            {
                status.Status = MessageStatus.Seen;
                status.ReadAt = DateTime.UtcNow;
                unitOfWork.MessageStatuses.Update(status);
            }
            await unitOfWork.SaveChangesAsync();
        }
    }
}
