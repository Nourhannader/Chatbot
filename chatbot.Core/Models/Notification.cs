using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Models
{
    public class Notification:BaseEntity
    {
        public Guid UserId { get; set; } 
        public ApplicationUser User { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationType Type { get; set; } 
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
