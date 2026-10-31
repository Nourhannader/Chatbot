using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public sealed class SearchResultDto
    {
        public SearchType Type { get; init; }

        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string? ImageUrl { get; init; }

        public Guid? ConversationId { get; init; }

        public DateTime? CreatedAt { get; init; }
    }
}
