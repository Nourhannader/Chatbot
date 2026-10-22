using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.MessageStatus
{
    public class MessageRecipientStatusDto
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public string? UserName { get; set; }

        // Fully qualify the MessageStatus type to resolve ambiguity
        public chatbot.Core.Enums.MessageStatus Status { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? ReadAt { get; set; }
    }
}
