using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class UpdateNotificationPreferencesDto
    {
        public bool NewMessages { get; set; }

        public bool MessageReactions { get; set; }
        public bool MessageReplies { get; set; }

        public bool Mentions { get; set; }

        public bool PushNotifications { get; set; }

        public bool SoundEnabled { get; set; }
    }
}
