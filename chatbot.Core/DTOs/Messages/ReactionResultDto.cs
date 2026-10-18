using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Messages
{
    public class ReactionResultDto
    {
        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public Guid ConversationId { get; set; }

        public ReactionType? ReactionType { get; set; }

        public ReactionAction Action { get; set; }
    }
}
