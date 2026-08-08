using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class MessageReactionDto
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Reaction { get; set; }
    }
}
