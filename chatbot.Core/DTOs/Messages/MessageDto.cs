using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Messages
{
    public class MessageDto
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }


        // Sender
        public Guid SenderId { get; set; }

        public string SenderName { get; set; }
            = string.Empty;


        // Message
        public string? Content { get; set; }

        public MessageType MessageType { get; set; }

        public DateTime CreatedAt { get; set; }


        // Edit
        public bool IsEdited { get; set; }

        public DateTime? EditedAt { get; set; }


        // Reply
        public ReplyPreviewDto? ReplyTo { get; set; }

        // Forward
        public bool IsForwarded { get; set; }


        // Delete For Everyone
        public bool IsDeletedForEveryone { get; set; }

        public DateTime? DeletedForEveryoneAt { get; set; }


        // Encryption
        public bool IsEncrypted { get; set; }

        public string? EncryptionVersion { get; set; }

        //voice note
        public VoiceNoteDto? VoiceNote { get; set; }

        //sticker

        public StickerDto? Sticker { get; set;}


        // System Message
        public bool IsSystemMessage { get; set; }

        public SystemMessageType? SystemMessageType { get; set; }


        // Files
        public List<FileMetadataDto> Files { get; set; }
            = new();


        // Reactions
        public List<MessageReactionDto> Reactions { get; set; }
            = new();


        // Status
        public List<MessageRecipientStatusDto> RecipientStatuses { get; set; }
            = new();

        public List<MessageAttachmentDto> attachments { get; set; } = new();


    }
}
