using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class EditMessageDto
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
