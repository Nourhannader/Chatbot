using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.DTOs
{
    public class SendVoiceNoteDto
    {
        public string ConversationId { get; set; }

        public IFormFile Audio { get; set; } = null!;

        public int DurationSeconds { get; set; }
    }
}
