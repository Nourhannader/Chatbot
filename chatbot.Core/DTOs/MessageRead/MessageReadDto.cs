using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.MessageRead
{
    public class MessageReadDto
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; } 

        public Guid UserId { get; set; } 

        public DateTime ReadAt { get; set; }
    }
}
