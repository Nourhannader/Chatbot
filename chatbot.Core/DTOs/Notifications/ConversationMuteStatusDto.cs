using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Notifications
{
    public class ConversationMuteStatusDto
    {
        public bool IsMuted { get; set; }

        public bool IsMutedForever { get; set; }

        public DateTime? MutedUntil { get; set; }
    }
}
