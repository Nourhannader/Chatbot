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
        public async Task<MessageStatus> GetStatusAsync(string messageId, string recipientId)
        {
            var status= await unitOfWork.MessageStatuses.GetAsync(messageId, recipientId);
            return status?.Status ?? MessageStatus.Sent;

        }

        public  async Task<List<MessageRecipientStatus>> GetStatusesAsync(string messageId)
        {
            return await unitOfWork.MessageStatuses.GetByMessageAsync(messageId);
        }

        public async Task MarkDeliveredAsync(string messageId, string recipientId)
        {
            var status = await unitOfWork.MessageStatuses.GetAsync(messageId, recipientId);
            if (status == null)
            {
                return;
            }
            if (status.Status >= MessageStatus.Delivered)
                return;
            status.Status = MessageStatus.Delivered;
            status.DeliveredAt = DateTime.UtcNow;
            unitOfWork.MessageStatuses.Update(status);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task MarkReadAsync(string messageId, string recipientId)
        {
            var status = await unitOfWork.MessageStatuses.GetAsync(messageId, recipientId);
            if (status == null)
            {
                return;
            }
            if (status.Status == MessageStatus.Read)
                return;
            status.Status = MessageStatus.Read;
            status.ReadAt = DateTime.UtcNow;
            unitOfWork.MessageStatuses.Update(status);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
