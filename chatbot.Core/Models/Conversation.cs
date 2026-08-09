using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Models
{
    public class Conversation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public ConversationType Type { get; set; } = ConversationType.OneToOne;

        [MaxLength(100)]
        public string? Title { get; set; }

        public string? GroupPictureUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //Created by user
        public string? CreatedById { get; set; }

        public ApplicationUser? CreatedBy { get; set; }

        // Navigation Properties
        public ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
