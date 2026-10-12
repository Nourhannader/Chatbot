using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.InviteGroup
{
    public class CreateGroupInviteDto
    {
        public int? MaxUses { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
