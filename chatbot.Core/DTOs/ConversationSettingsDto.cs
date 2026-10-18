using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;

namespace chatbot.Core.DTOs
{
    public class ConversationSettingsDto
    {
        public bool IsMuted { get; set; }

        public DateTime? MuteUntil { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? ArchivedAt { get; set; }

        public bool IsPinned { get; set; }

        public DateTime? PinnedAt { get; set; }
    }
}
