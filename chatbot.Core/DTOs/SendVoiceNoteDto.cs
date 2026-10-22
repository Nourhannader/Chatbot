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
        public Guid ConversationId { get; set; }

        public IFormFile File { get; set; }
            = null!;

        public Guid? ReplyToMessageId { get; set; }
    }
}
