using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Messages
{
    public class MessageReactionDto
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }
            = string.Empty;

        public ReactionType ReactionType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
