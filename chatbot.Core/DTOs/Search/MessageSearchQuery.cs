using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Search
{
    public sealed class MessageSearchQuery
    {
        public string Keyword { get; init; } = string.Empty;

        public Guid? ConversationId { get; init; }

        public Guid? SenderId { get; init; }

        public MessageType? MessageType { get; init; }

        public DateTime? From { get; init; }

        public DateTime? To { get; init; }

        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 20;
    }
}
