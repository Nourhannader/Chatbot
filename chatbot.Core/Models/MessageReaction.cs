using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chatbot.Core.Models
{
    public class MessageReaction
    {
        public int Id { get; set; }
        [ForeignKey("Message")]
        public long MessageId { get; set; }
        public Message Message { get; set; }

        [ForeignKey("user")]
        public string UserId { get; set; }

        public ApplicationUser user { get; set; }

        public string ReactionType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
