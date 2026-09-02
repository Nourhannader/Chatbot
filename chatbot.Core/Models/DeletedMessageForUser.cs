using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class DeletedMessageForUser: BaseEntity
    {
        public Guid MessageId { get; set; } 
        public Message Message { get; set; } = null!;

        public Guid UserId { get; set; } 
        public ApplicationUser User { get; set; } = null!;

        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
}
