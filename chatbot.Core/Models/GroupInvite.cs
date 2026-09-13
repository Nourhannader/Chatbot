using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class GroupInvite:BaseEntity
    {
        public Guid ConversationId { get; set; } 

        public Conversation Conversation { get; set; } = null!;

        public string Code { get; set; } = Guid.NewGuid().ToString("N");

        public bool IsActive { get; set; } = true;

        public DateTime ExpiresAt { get; set; }

        public int MaxUses { get; set; } = 100;

        public int UsedCount { get; set; }
    }
}
