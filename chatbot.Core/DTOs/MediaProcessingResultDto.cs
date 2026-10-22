using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class MediaProcessingResultDto
    {
        public int? Width { get; set; }

        public int? Height { get; set; }

        public double? DurationSeconds { get; set; }

        public Stream? ThumbnailStream { get; set; }

        public string? ThumbnailContentType { get; set; }

        public List<double>? Waveform { get; set; }
    }
}
