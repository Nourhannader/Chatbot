using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;


namespace chatbot.Core.Models
{
    
    public class Message
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ConversationId { get; set; } = string.Empty;
        public Conversation Conversation { get; set; } = null!;

        [Required]
        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser Sender { get; set; } = null!;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public MessageType Type { get; set; } = MessageType.Text;

        public string? FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public int? FileDurationSeconds { get; set; }
        public string? FileUrl { get; set; }
        //edited Message Feature
        public DateTime? EditedAt { get; set; }

        public bool IsEdited => EditedAt != null;

        // Delete For Everyone Feature
        public bool IsDeletedForEveryone { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        //Message Reply Feature
        public string? ReplyToMessageId { get; set; }

        public Message? ReplyToMessage { get; set; }
        //forwarded Message Feature
        public bool IsForwarded { get; set; }

        public string? OriginalMessageId { get; set; }
        public Message? OriginalMessage { get; set; }

        //navigation property for message forwardies
        public ICollection<Message> ForwardMessages { get; set; } = new List<Message>();

        //navigation property for message replies
        public ICollection<Message> Replies { get; set; } = new List<Message>();

        // Navigation Property for Delete For Me Feature
        public ICollection<DeletedMessageForUser> DeletedForUsers { get; set; } = new List<DeletedMessageForUser>();

        // Navigation Property for Recipient Statuses
        public ICollection<MessageRecipientStatus> RecipientStatuses { get; set; } = new List<MessageRecipientStatus>();
        public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
    }
}

