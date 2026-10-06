using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.UserConnection
{
    public class UserConnectionDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserDeviceId { get; set; }

        public string ConnectionId { get; set; } = string.Empty;

        public bool IsOnline { get; set; }

        public DateTime ConnectedAt { get; set; }

        public DateTime? DisconnectedAt { get; set; }
    }
}
