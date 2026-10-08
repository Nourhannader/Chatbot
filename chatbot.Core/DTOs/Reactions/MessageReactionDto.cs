using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Reactions
{
    public class MessageReactionDto
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public ReactionType ReactionType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
