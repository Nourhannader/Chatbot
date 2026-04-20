using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class ChatbotResponse
    {
        public int Id { get; set; }
        public string Pattern { get; set; }
        public string ResponseText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
