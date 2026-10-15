using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public Conversation Conversation { get; set; }
            = null!;


        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }
            = null!;


        // =============================
        // Role
        // =============================

        public ConversationRole Role { get; set; }
            = ConversationRole.Member;


        // =============================
        // Membership
        // =============================

        public DateTime JoinedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? LeftAt { get; set; }


        // =============================
        // Read state
        // =============================

        public DateTime? LastReadAt { get; set; }

        public Guid? LastReadMessageId { get; set; }


        // =============================
        // Admin Mute
        // =============================

        public bool IsMutedByAdmin { get; set; }

        public DateTime? MutedUntil { get; set; }

        public Guid? MutedById { get; set; }

        public ApplicationUser? MutedBy { get; set; }


        // =============================
        // Ban
        // =============================

        public bool IsBanned { get; set; }

        public DateTime? BannedAt { get; set; }

        public Guid? BannedById { get; set; }

        public ApplicationUser? BannedBy { get; set; }

        [MaxLength(500)]
        public string? BanReason { get; set; }

    }
}
