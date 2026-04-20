using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Ef.Data;

namespace chatbot.Core.Models
{
    public class UserDevice
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string ConnectionId { get; set; }
        public bool IsConnected { get; set; }

        public DateTime LastSeen { get; set; }
    }
}
