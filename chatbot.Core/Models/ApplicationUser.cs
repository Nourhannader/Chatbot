using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chatbot.Core.Models
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser
    {

        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        public string? ImageProfileUrl { get; set; }
        public string? Bio { get; set; }

        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

        public ICollection<ChatMembers> ChatMembers { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<UserDevice> Devices { get; set; }
    }
}
