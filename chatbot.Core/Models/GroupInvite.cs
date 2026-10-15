using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class GroupInvite:BaseEntity
    {
        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; }
            = null!;


        [Required]
        [MaxLength(100)]
        public string Code { get; set; }
            = null!;


        public bool IsActive { get; set; }
            = true;


        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public Guid CreatedById { get; set; }

        public ApplicationUser CreatedBy { get; set; }
            = null!;


        public DateTime ExpiresAt { get; set; }

        public int MaxUses { get; set; }
            = 100;

        public int UsedCount { get; set; }


        // =============================
        // Revoke
        // =============================

        public DateTime? RevokedAt { get; set; }

        public Guid? RevokedById { get; set; }

        public ApplicationUser? RevokedBy { get; set; }


        // =============================
        // Concurrency
        // =============================

        [Timestamp]
        public byte[] RowVersion { get; set; }
            = null!;
    }
}
