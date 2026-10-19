using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Notifications
{
    public class UserDeviceDto
    {
        public Guid Id { get; set; }

        public DeviceType DeviceType { get; set; }

        public PushProvider Provider { get; set; }

        public string? DeviceName { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUsedAt { get; set; }
    }
}
