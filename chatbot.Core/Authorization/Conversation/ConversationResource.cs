using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Authorization.Conversation
{
    public class ConversationResource
    {
        public Guid ConversationId { get; }

        public ConversationResource(Guid conversationId)
        {
            ConversationId = conversationId;
        }
    }
}
