using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.MessageStatus
{
    public class MarkReadDto
    {
        public Guid ConversationId { get; set; }

        public Guid LastMessageId { get; set; }
    }
}
