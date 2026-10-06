using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.UserConnection
{
    public class UserConnectionStatusDto
    {
        public Guid UserId { get; set; }

        public bool IsOnline { get; set; }

        public int ActiveConnections { get; set; }
    }
}
