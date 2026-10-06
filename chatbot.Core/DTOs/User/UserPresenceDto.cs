using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.User
{
    public class UserPresenceDto
    {
        public Guid UserId { get; set; } 

        public bool IsOnline { get; set; }

        public int ConnectionCount { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}
