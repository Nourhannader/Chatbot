using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Models
{
    public class StoredFile: BaseEntity
    {

        public Guid MessageId { get; set; }

        public Message Message { get; set; }
            = null!;

        public string OriginalName { get; set; }
            = string.Empty;


        public string StoredName { get; set; }
            = string.Empty;


        public string Path { get; set; }
            = string.Empty;


        public string ContentType { get; set; }
            = string.Empty;


        public long Size { get; set; }


        public StorageProviderType Provider { get; set; }


        public string? ThumbnailPath { get; set; }


        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? DurationSeconds { get; set; }


        // Soft Delete
        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsPhysicallyDeleted { get; set; }


        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public string UploadedByUser { get; set; }
            = string.Empty;
    }
}
