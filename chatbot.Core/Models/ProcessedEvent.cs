using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class ProcessedEvent
    {
        public Guid EventId { get; set; }

        public string HandlerName { get; set; }
            = null!;

        public DateTime ProcessedAt { get; set; }
            = DateTime.UtcNow;
    }
}
