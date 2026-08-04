using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class BlockList
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Blocker")]
        public string BlockerId { get; set; }
        public ApplicationUser Blocker { get; set; }
        [ForeignKey("Blocked")]
        public string BlockedId { get; set; }
        public ApplicationUser Blocked { get; set; }
        public DateTime BlockedAt { get; set; }
    }
}
