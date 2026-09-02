using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class BlockList : BaseEntity
    {
        [ForeignKey("Blocker")]
        public Guid BlockerId { get; set; }
        public ApplicationUser Blocker { get; set; }
        [ForeignKey("Blocked")]
        public Guid BlockedId { get; set; }
        public ApplicationUser Blocked { get; set; }
        public DateTime BlockedAt { get; set; }
    }
}
