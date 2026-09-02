using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chatbot.Core.Models
{
   
    public class RefreshToken : BaseEntity
    {

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

        public bool IsActive => RevokedOn == null && !IsExpired;

        public Guid UserId { get; set; } 

        public ApplicationUser User { get; set; } = null!;
    }
}
