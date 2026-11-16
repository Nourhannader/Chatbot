using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.MessageStatus
{
    public sealed class MessageStatusDto
    {
        public Guid MessageId { get; set; }

        public Guid ConversationId { get; set; }

        public Guid UserId { get; set; }

        public chatbot.Core.Enums.MessageStatus Status { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
