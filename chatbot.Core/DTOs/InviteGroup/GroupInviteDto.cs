using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.InviteGroup
{
    public class GroupInviteDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int MaxUses { get; set; }
        public int UsedCount { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public int RemaningUses => Math.Max(0, MaxUses - UsedCount);
    }
}
