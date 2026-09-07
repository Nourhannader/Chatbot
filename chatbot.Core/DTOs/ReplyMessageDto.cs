using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class ReplyMessageDto
    {
        public Guid ConversationId { get; set; } 

        public Guid MessageId { get; set; } 

        public string Content { get; set; } = string.Empty;

        public MessageType Type { get; set; }
    }
}
