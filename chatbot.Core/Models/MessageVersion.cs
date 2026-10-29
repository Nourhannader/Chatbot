using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class MessageVersion : BaseEntity
    {
        public Guid MessageId { get; set; }

        public Message Message { get; set; } = null!;


        public string Content { get; set; }
            = string.Empty;


        public int Version { get; set; }


        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}
