using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class VoiceNote:BaseEntity
    {
       
        public Guid MessageId { get; set; }

        public Message Message { get; set; }
            = null!;


        public Guid FileId { get; set; }

        public StoredFile File { get; set; }
            = null!;


        public int DurationSeconds { get; set; }


        public string Waveform { get; set; }
            = string.Empty;


        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}
