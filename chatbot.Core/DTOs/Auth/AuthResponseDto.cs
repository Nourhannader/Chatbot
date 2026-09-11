using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
            = string.Empty;

        public string RefreshToken { get; set; }
            = string.Empty;
    }
}
