using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class NotificationOptions
    {
        public bool SaveToDatabase { get; set; } = true;

        public bool SendRealtimeNotification { get; set; } = true;

        public bool SendPush { get; set; } = true;

        public bool SoundEnabled { get; set; } = true;

        public Guid? ConversationId { get; set; }

        public Guid? MessageId { get; set; }

        public Dictionary<string, string>? Data { get; set; }
    }
}
