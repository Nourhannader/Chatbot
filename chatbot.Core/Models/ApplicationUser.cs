using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace chatbot.Core.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public bool ReadReceiptsEnabled { get; set; }
        public bool LastSeenVisible { get; set; }
        public bool IsTypingVisible { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<ConversationMember> ConversationMembers { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<UserDevice> Devices { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<UserConnection> Connections { get; set; }
        public ICollection<MessageReaction> MessageReactions { get; set; }
        public ICollection<MessageRecipientStatus> MessageRecipientStatuses { get; set; }
        public ICollection<BlockList> BlockedUsers { get; set; }
        public ICollection<BlockList> BlockedByUsers { get; set; }
        public ICollection<DeletedMessageForUser> DeletedMessages { get; set; }
        public ICollection<StoredFile> UploadedFiles { get; set; }
    }
}
