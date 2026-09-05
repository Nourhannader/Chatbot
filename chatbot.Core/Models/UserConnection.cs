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

        public ApplicationUser User { get; set; }
            = null!;

        public Guid? UserDeviceId { get; set; }

        public UserDevice? UserDevice { get; set; }

        public string ConnectionId { get; set; }
            = string.Empty;

        public bool IsOnline { get; set; }

        public DateTime ConnectedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? DisconnectedAt { get; set; }
    }
}
