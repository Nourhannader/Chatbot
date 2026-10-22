using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class VoiceNoteDto
    {
        public Guid Id { get; set; }

        public Guid FileId { get; set; }

        public string FileUrl { get; set; }
            = string.Empty;

        public double DurationSeconds { get; set; }

        public List<double> Waveform { get; set; }
            = new();

        public FileProcessingStatus ProcessingStatus { get; set; }
    }
}
