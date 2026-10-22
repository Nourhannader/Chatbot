using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class VoiceNote:BaseEntity
    {
        // ==============================
        // Message
        // ==============================

        public Guid MessageId { get; set; }

        public Message Message { get; set; }
            = null!;


        // ==============================
        // File
        // ==============================

        public Guid FileId { get; set; }

        public StoredFile File { get; set; }
            = null!;


        // ==============================
        // Voice Metadata
        // ==============================

        public double DurationSeconds { get; set; }

        public string Waveform { get; set; }
            = "[]";


        // ==============================
        // Time
        // ==============================

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}
