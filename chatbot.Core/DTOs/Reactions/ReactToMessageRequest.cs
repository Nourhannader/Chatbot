using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Reactions
{
    public class ReactToMessageRequest
    {
        public ReactionType Type { get; set; }
    }
}
