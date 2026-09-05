using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Models
{
    public class UserDevice : BaseEntity
    {
        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }
            = null!;

        public string DeviceToken { get; set; }
            = string.Empty;

        public DeviceType DeviceType { get; set; }

        public string? DeviceName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime LastLogin { get; set; }
            = DateTime.UtcNow;

        public ICollection<UserConnection> Connections { get; set; }
            = new List<UserConnection>();
    }
}
