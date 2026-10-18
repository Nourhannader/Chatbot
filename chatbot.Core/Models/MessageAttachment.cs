using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class MessageAttachment : BaseEntity
    {
        // ==============================
        // Message
        // ==============================

        public Guid MessageId { get; set; }

        public Message Message { get; set; }
            = null!;


        // ==============================
        // Stored File
        // ==============================

        public Guid StoredFileId { get; set; }

        public StoredFile StoredFile { get; set; }
            = null!;


        // ==============================
        // Attachment Metadata
        // ==============================

        public int Order { get; set; }

        public string? Caption { get; set; }
    }
}
