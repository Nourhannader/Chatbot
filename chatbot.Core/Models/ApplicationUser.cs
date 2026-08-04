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
        //privacy settings
        public bool ReadReceiptsEnabled { get; set; } = true;

        public bool LastSeenVisible { get; set; } = true;

        public bool IsTypingVisible { get; set; } = true;
        public ICollection<RefreshToken>? RefreshTokens { get; set; } 

        public ICollection<ConversationMember> ConversationMembers { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<UserDevice> Devices { get; set; }
    }
}
