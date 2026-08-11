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
        public string ConversationId { get; set; } = string.Empty;

        public string ReplyToMessageId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public MessageType Type { get; set; }

        public string? FileUrl { get; set; }

        public string? FileName { get; set; }
    }
}
