using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Auth
{
    public class LoginDto
    {
        public string Email { get; set; }
            = string.Empty;

        public string Password { get; set; }
            = string.Empty;

        public string DeviceId { get; set; }
            = string.Empty;

        public DeviceType DeviceType { get; set; }

        public string? DeviceName { get; set; }
    }
}
