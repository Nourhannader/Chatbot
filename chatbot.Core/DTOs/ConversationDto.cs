using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.DTOs
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public ConversationType Type { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public ConversationImageDto? Image { get; set; }

        public LastMessageDto? LastMessage { get; set; }

        public int UnreadCount { get; set; }

        public ConversationRole? CurrentUserRole { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
