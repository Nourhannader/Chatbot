using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class NotificationOptions
    {
        public bool SendPush { get; set; } = true;

        public bool SendSignalR { get; set; } = true;

        public bool SaveToDatabase { get; set; } = true;

        public string? ConversationId { get; set; }

        public string? MessageId { get; set; }
        public Dictionary<string,string>? Data { get; set; } 
    }
}
