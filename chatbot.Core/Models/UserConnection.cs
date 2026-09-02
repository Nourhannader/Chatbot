using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class UserConnection : BaseEntity
    {
        public Guid UserId { get; set; } 
        public ApplicationUser User { get; set; } = null!;

        public Guid ConnectionId { get; set; } 

        public string DeviceType { get; set; } = string.Empty;

        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    }
}
