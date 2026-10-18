using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class FileMetadataDto
    {
        public Guid Id { get; set; }

        public string OriginalName { get; set; }
            = string.Empty;

        public string ContentType { get; set; }
            = string.Empty;

        public long Size { get; set; }

        public string? FileUrl { get; set; }

        public string? ThumbnailUrl { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public double? DurationSeconds { get; set; }
    }
}
