using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Helper
{
    public static class ConversationRoleHierarchy
    {
        public static int GetLevel(ConversationRole role)
        {
            return role switch
            {
                ConversationRole.Member => 1,
                ConversationRole.Admin => 2,
                ConversationRole.Owner => 3,

                _ => 0
            };
        }

        public static bool IsHigherThan(ConversationRole actor, ConversationRole target)
        {
            return GetLevel(actor) > GetLevel(target);
        }
    }
}
