using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Messages
{
    public class SendMessageDto
    {
        public Guid ConversationId { get; set; }

        public string Content { get; set; }
            = string.Empty;

        public MessageType MessageType { get; set; }
            = MessageType.Text;

        // Optional: message is a reply
        public Guid? ReplyToMessageId { get; set; }

        public DateTime? SendAt { get; set; }
    }
}
