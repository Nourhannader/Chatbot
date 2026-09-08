using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class ConversationSearchDto
    {
        public string Id {  get; set; }
        public string Title {  get; set; }
        public DateTime? LastMessageAt {  get; set; }
    }
}
