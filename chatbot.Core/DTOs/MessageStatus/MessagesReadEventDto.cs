using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.MessageStatus
{
    public class MessagesReadEventDto
    {
        public Guid UserId { get; set; }

        public Guid ConversationId { get; set; }

        public Guid LastMessageId { get; set; }

        public DateTime ReadAt { get; set; }
    }
}
