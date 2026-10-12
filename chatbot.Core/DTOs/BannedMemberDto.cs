using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class BannedMemberDto
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string? DisplayName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
       
        public DateTime? BannedAt { get; set; }
        public Guid? BannedById { get; set; }
    }
}
