using chatbot.Core.Models; // Add this using directive

namespace chatbot.Ef.Data
{
    public class ApplicationUser : IdentityUser
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

        public ICollection<ChatMember> ChatMembers { get; set; } 
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<UserDevice> Devices { get; set; }
    }
}
