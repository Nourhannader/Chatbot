using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class UploadSession:BaseEntity
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
            = null!;
        public Guid MessageId { get; set; }
        public Message Message { get; set; }
            = null!;

        public string OriginalFileName { get; set; }
            = string.Empty;

        public string ContentType { get; set; }
            = string.Empty;

        public long TotalSize { get; set; }

        public int TotalChunks { get; set; }

        public int UploadedChunks { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }
    }
}
