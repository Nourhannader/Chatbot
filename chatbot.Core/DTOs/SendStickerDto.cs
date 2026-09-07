using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class SendStickerDto
    {
        public Guid ConversationId { get; set; }

        public Guid StickerId { get; set; }
    }
}
