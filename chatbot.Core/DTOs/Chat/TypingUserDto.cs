using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Chat
{
    public sealed class TypingUserDto
    {
        public Guid ConversationId { get; set; }

        public Guid UserId { get; set; }
    }
}
