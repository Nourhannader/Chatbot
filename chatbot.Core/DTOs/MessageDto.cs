using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.DTOs
{
    public class MessageDto
    {
        public string Id { get; set; }

        public string SenderId { get; set; }

        public string SenderName { get; set; }

        public string Content { get; set; }

        public MessageType Type { get; set; }

        public DateTime SentAt { get; set; }

        public string? FileUrl { get; set; }

        public bool IsDeletedForEveryone { get; set; }

        public List<MessageReactionDto> Reactions { get; set; } = [];
    }
}
