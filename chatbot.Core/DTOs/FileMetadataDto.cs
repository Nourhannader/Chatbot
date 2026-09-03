using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class FileMetadataDto
    {
        public string Id { get; set; }

        public string OriginalName { get; set; }
            = string.Empty;

        public string ContentType { get; set; }
            = string.Empty;

        public long Size { get; set; }

        public string FileUrl { get; set; }
            = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? DurationSeconds { get; set; }
    }
}
