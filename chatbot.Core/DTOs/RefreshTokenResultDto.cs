using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class RefreshTokenResultDto
    {
        public string AccessToken { get; set; }
            = string.Empty;

        public string RefreshToken { get; set; }
            = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}
