using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class chat
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<ChatMembers> Members {  get; set; }
        public ICollection<Message> Messages { get; set; }

    }
}
