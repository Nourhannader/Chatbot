using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class DeleteMessageResultDto
    {
        public Guid MessageId { get; set; }

        public Guid ConversationId { get; set; }

        public Guid DeletedByUserId { get; set; }

        public DateTime DeletedAt { get; set; }
    }
}
