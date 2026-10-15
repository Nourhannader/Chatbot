using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class ConversationMemberDetailsDto :ConversationMemberDto
    {
        public DateTime? LeftAt { get; set; }

        public DateTime? LastReadAt { get; set; }

        public bool IsBanned { get; set; }

        public DateTime? BannedAt { get; set; }

        public string? BanReason { get; set; }
    }
}
