using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Models
{
    public class DeviceSession:BaseEntity
    {

        public Guid UserId { get; set; } 

        public ApplicationUser User { get; set; } = null!;

        public string DeviceName { get; set; } = string.Empty;

        public DeviceType DeviceType { get; set; } 

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string RefreshTokenHash { get; set; } = string.Empty;

        public DateTime RefreshTokenExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    }
}
