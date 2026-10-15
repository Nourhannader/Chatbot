using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class ConversationUserSettings:BaseEntity
    {
        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; }
            = null!;


        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }
            = null!;


        // =============================
        // Notification mute
        // =============================

        public bool IsMuted { get; set; }

        public DateTime? MuteUntil { get; set; }


        // =============================
        // Archive
        // =============================

        public bool IsArchived { get; set; }

        public DateTime? ArchivedAt { get; set; }


        // =============================
        // Pin
        // =============================

        public bool IsPinned { get; set; }

        public DateTime? PinnedAt { get; set; }
    }
}
