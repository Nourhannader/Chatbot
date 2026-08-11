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
        public string Id { get; set; }

        public string? Title { get; set; }

        public ConversationType Type { get; set; }

        public string? GroupPictureUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public MessageDto? LastMessage { get; set; }

        public int UnReadCount { get; set; }

        public List<UserDto> Members { get; set; } = [];
    }
}
