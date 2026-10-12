using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using ServiceStack.DataAnnotations;

namespace chatbot.Core.Models
{
    public class ConversationMember:BaseEntity
    {
       
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        
        public Guid UserId { get; set; } 
        public ApplicationUser User { get; set; } = null!;

        public ConversationRole Role { get; set; } = ConversationRole.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }

        // Real-Time Unread Count Tracking
        public DateTime? LastReadAt { get; set; }

        //Moderation Feature
        public bool IsBanned { get; set; }
        public DateTime? BannedAt { get; set; }

        public Guid? BannedById { get; set; }

        public ApplicationUser? BannedBy { get; set; }

        public bool IsMutedByAdmin { get; set; }

        public DateTime? MutedUntil { get; set; }

        public Guid? MutedById { get; set; }

        public ApplicationUser? MutedBy { get; set; }

    }
}
