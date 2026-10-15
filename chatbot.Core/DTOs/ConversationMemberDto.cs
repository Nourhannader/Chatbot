using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class ConversationMemberDto
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }
            = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public ConversationRole Role { get; set; }

        public DateTime JoinedAt { get; set; }

        public bool IsMutedByAdmin { get; set; }

        public DateTime? MutedUntil { get; set; }
    }
}
