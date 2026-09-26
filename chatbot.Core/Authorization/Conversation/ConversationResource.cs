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
        public Guid? MessageId {  get; }
        public Guid? TargetUserId {  get; }


        public ConversationResource(Guid conversationId, Guid? messageId=null, Guid? targetUserId= null)
        {
            ConversationId = conversationId;
            MessageId = messageId;
            TargetUserId = targetUserId;
        }
    }
}
