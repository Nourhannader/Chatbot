using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class VoiceNoteDto
    {
        public Guid Id { get; set; }

        public double DurationSeconds { get; set; }

        public string? WaveformData { get; set; }
    }
}
