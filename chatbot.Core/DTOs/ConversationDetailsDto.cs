using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class ConversationDetailsDto
    {
        public Guid Id { get; set; }

        public ConversationType Type { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public ConversationImageDto? Image { get; set; }

        public int MembersCount { get; set; }

        public ConversationRole? CurrentUserRole { get; set; }

        public List<ConversationMemberDto> Members { get; set; } = [];

        public DateTime CreatedAt { get; set; }
    }
}
