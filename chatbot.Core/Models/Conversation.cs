using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Models
{
    public class Conversation:BaseEntity
    {
        public ConversationType Type { get; set; }

        // Mainly for groups
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;


        // =============================
        // Creator
        // =============================

        public Guid? CreatedById { get; set; }

        public ApplicationUser? CreatedBy { get; set; }


        // =============================
        // Image
        // =============================

        public Guid? ImageId { get; set; }

        public StoredFile? Image { get; set; }


        // =============================
        // Private conversation
        // =============================

        [MaxLength(100)]
        public string? PrivateConversationKey { get; set; }


        // =============================
        // Soft Delete
        // =============================

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedById { get; set; }

        public ApplicationUser? DeletedBy { get; set; }


        // =============================
        // Navigation
        // =============================

        public ICollection<ConversationMember> Members { get; set; }
            = new List<ConversationMember>();

        public ICollection<Message> Messages { get; set; }
            = new List<Message>();

        public ICollection<ConversationUserSettings> UserSettings { get; set; }
            = new List<ConversationUserSettings>();

        public ICollection<GroupInvite> GroupInvites { get; set; }
            = new List<GroupInvite>();
    }
}
