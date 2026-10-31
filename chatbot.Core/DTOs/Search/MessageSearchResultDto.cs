using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Search
{
    public sealed class MessageSearchResultDto
    {
        public Guid MessageId { get; init; }

        public Guid ConversationId { get; init; }

        public Guid SenderId { get; init; }

        public string? SenderName { get; init; }

        public string? Content { get; init; }

        public MessageType MessageType { get; init; }

        public DateTime CreatedAt { get; init; }

        public int MatchCount { get; init; }
    }
}
