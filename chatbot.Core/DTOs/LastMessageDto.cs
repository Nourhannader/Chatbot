using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class LastMessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }

        public string? SenderName { get; set; } 

        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }

        public DateTime SendAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
