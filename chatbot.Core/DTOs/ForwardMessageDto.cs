using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class ForwardMessageDto
    {
        public string MessageId { get; set; } = string.Empty;

        public List<string> ConversationIds { get; set; }
            = new();
    }
}
