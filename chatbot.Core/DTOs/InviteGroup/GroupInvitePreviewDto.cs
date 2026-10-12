using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.InviteGroup
{
    public class GroupInvitePreviewDto
    {
        public Guid ConversationId { get; set; }
        public string GroupName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int MemberCount { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired { get; set; }
        public bool CanJoin{ get; set; }

    }
}
