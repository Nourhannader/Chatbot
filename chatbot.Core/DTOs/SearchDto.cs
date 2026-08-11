using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class SearchDto
    {
        public string Keyword { get; set; } = string.Empty;

        public SearchType SearchType { get; set; }

        public string? ConversationId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
