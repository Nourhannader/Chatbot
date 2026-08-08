using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string? ImageProfileUrl { get; set; }

        public bool IsOnline { get; set; }

        public DateTime? LastSeen { get; set; }
    }
}
