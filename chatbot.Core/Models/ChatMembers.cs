using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class ChatMembers
    {
        public int Id { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public chat Chat { get; set; }
        [ForeignKey("user")]
        public string UserId { get; set; }
       
        public ApplicationUser user {  get; set; }
        public bool IsAdmin { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
