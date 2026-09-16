using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.DTOs
{
    public class CreateMessageDto
    {
        public string ConversationId { get; set; }
        = string.Empty;

        public string Content { get; set; }
            = string.Empty;

        public MessageType Type { get; set; }
        public IFormFile File { get; set; }
    }
}
