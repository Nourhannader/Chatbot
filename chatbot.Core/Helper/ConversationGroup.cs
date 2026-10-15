using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Helper
{
    public static class ConversationGroup
    {
        public static string Name(
            Guid conversationId)
        {
            return $"conversation:{conversationId:N}";
        }
    }
}
