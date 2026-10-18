using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class ForwardMessageDto
    {
        public Guid MessageId { get; set; }

        public List<Guid> ConversationIds { get; set; }
            = new();
    }
}
