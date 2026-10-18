using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;


namespace chatbot.Core.Models
{
    public class MessageReaction:BaseEntity
    {
        public Guid MessageId { get; set; } 
        public Message Message { get; set; } = null!;

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public  ReactionType ReactionType{ get; set; } 
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
