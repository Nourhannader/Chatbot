using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Ef.Data;

namespace chatbot.Core.Models
{
    public enum MessageType
    {
        Text = 0,
        Image = 1,
        Video = 2,
        File = 3,
        Voice = 4
    }
    public class Message
    {
        public long Id { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public chat chat { get; set; }
        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }

        public string? MessageText { get; set; }

        public MessageType MessageType { get; set; } 
        public string? MediaUrl { get; set; }
        [ForeignKey("ReplyTo")]
        public long? ReplyToMessageId { get; set; }
        public Message ReplyTo { get; set; }

        public DateTime SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }

        public bool IsDeletedBySender { get; set; }
        public bool IsDeletedForEveryone { get; set; }

        public ICollection<MessageReaction> Reactions { get; set; }
    }
}
