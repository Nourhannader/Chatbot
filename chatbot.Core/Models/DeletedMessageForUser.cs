using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class DeletedMessageForUser
    {
        public string MessageId { get; set; } = string.Empty;
        public Message Message { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
}
