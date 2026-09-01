using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class UserConnection : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string ConnectionId { get; set; } = string.Empty;

        public string DeviceType { get; set; } = string.Empty;

        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    }
}
