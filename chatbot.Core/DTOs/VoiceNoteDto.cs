using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class VoiceNoteDto
    {
        public string Id { get; set; }

        public string FileId { get; set; }

        public int DurationSeconds { get; set; }

        public string Waveform { get; set; }
            = string.Empty;
    }
}
