using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chatbot.Core.Models
{

    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }
            = null!;

        public Guid DeviceSessionId { get; set; }

        public DeviceSession DeviceSession { get; set; }
            = null!;

        public string TokenHash { get; set; }
            = string.Empty;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public string? CreatedByIp { get; set; }

        public string? RevokedByIp { get; set; }

        public bool IsExpired =>
            DateTime.UtcNow >= ExpiresAt;

        public bool IsRevoked =>
            RevokedAt.HasValue;

        public bool IsActive =>
            !IsRevoked && !IsExpired;
    }
}
