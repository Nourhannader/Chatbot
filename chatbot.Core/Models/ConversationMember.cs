using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace chatbot.Core.Models
{
    public class ConversationMember:BaseEntity
    {
       
        public string ConversationId { get; set; } = string.Empty;
        public Conversation Conversation { get; set; } = null!;
        
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public bool IsAdmin { get; set; } = false;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }

        // Real-Time Unread Count Tracking
        public DateTime? LastReadAt { get; set; }

        // Mute Feature
        public DateTime? MutedUntil { get; set; }

        // Pinning Feature
        public bool IsPinned { get; set; } = false;
        public DateTime? PinnedAt { get; set; }
        //Archive Feature
        public bool IsArchived { get; set; }

        public DateTime? ArchivedAt { get; set; }
    }
}
