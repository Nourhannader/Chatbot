using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class SearchRequestDto
    {
        public string Keyword { get; set; } = string.Empty;

        public SearchType Type { get; set; } = SearchType.All;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        // Message filters
        public Guid? ConversationId { get; set; }

        public Guid? SenderId { get; set; }

        public MessageType? MessageType { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
