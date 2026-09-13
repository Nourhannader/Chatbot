using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class NotificationPreferences:BaseEntity
    {

        public Guid UserId { get; set; } 

        public ApplicationUser User { get; set; } = null!;

        public bool NewMessages { get; set; } = true;

        public bool MessageReactions { get; set; } = true;
        public bool MessageReplies {  get; set; } = true;


        public bool Mentions { get; set; } = true;

        public bool PushNotifications { get; set; } = true;

        public bool SoundEnabled { get; set; } = true;
    }
}
