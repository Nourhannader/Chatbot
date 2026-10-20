using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Notifications
{
    public class RemoveDeviceDto
    {
        public string PushToken { get; set; } = string.Empty;
        public PushProvider Provider { get; set; }
    }
}
