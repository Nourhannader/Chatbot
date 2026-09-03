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
    
    public class Message:BaseEntity
    {

        public Guid SenderId { get; set; }

        public ApplicationUser Sender { get; set; }
            = null!;

        
        public Guid ConversationId { get; set; }

        public Conversation Conversation { get; set; }
            = null!;


        // Message content
        public string Content { get; set; }
            = string.Empty;


        public MessageType MessageType { get; set; }


        // Multiple Files
        public ICollection<StoredFile> Files { get; set; }
            = new List<StoredFile>();


        // Voice Note Metadata
        public VoiceNote? VoiceNote { get; set; }


        // Reply
        public Guid? ReplyToMessageId { get; set; }

        public Message? ReplyToMessage { get; set; }


        // Edit
        public bool IsEdited { get; set; }

        public DateTime? EditedAt { get; set; }


        // Delete
        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }


        // Time
        public DateTime SendAt { get; set; }
            = DateTime.UtcNow;

        // Delete For Everyone Feature
        public bool IsDeletedForEveryone { get; set; } = false;
        //forwarded Message Feature
        public bool IsForwarded { get; set; }

        public Guid? OriginalMessageId { get; set; }
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
        public ICollection<StoredFile> StoredFiles { get; set; } = new List<StoredFile>();
    }
}

