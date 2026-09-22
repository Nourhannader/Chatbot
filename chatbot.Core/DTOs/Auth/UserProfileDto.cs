using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Auth
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Bio { get; set; }

        public string? ProfileImageUrl { get; set; }

        public bool IsOnline { get; set; }

        public DateTime? LastSeenAt { get; set; }
    }
}
