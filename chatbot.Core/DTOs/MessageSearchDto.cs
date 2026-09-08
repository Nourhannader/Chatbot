using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class MessageSearchDto
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string ConversationId { get; set; }

        public string SenderId { get; set; }

        public MessageType Type { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
