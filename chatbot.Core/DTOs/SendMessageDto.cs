using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.DTOs
{
    public class SendMessageDto
    {
        public Guid ConversationId { get; set; } 

        public string Content { get; set; } = string.Empty;
    }
}
