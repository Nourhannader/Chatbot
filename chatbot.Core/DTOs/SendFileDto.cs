using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class SendFileDto
    {

        public string UserId { get; set; }

        public string ConversationId { get; set; }

        public string Caption { get; set; }

        public MessageType Type { get; set; }

        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? FileUrl { get; set; }
    }
}
