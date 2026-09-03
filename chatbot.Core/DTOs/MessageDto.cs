using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.DTOs
{
    public class MessageDto
    {
        public string Id { get; set; }

        public string ConversationId { get; set; }

        public string SenderId { get; set; }

        public string? SenderName { get; set; }

        public string Content { get; set; }
            = string.Empty;

        public MessageType MessageType { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<FileMetadataDto> Files { get; set; }
            = [];

        public VoiceNoteDto? VoiceNote { get; set; }

        public bool IsEdited { get; set; }

        public DateTime? EditedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
