using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Models
{
    public class MessageRecipientStatus : BaseEntity
    {
        public string MessageId { get; set; } = string.Empty;
        public Message Message { get; set; } = null!;
        public string RecipientId { get; set; } = string.Empty;
        public ApplicationUser Recipient { get; set; } = null!;

        public MessageStatus Status { get; set; } = MessageStatus.Sent;

        public DateTime? ReadAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
