using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class UserDevice
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string UserId { get; set; }=string.Empty;
        public ApplicationUser User { get; set; } = null!;

        //Firebase 
        public string DeviceToken { get; set; }=string.Empty;
        //Andriod, iOS, Web
        public string DeviceType { get; set; } = string.Empty;
        public string? DeviceName { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastLogin { get; set; }
        //signalR connection id for real-time communication
        public string? ConnectionId { get; set; }

        public bool IsOnline { get; set; }

        public DateTime ConnectedAt { get; set; }

        public DateTime? DisconnectedAt { get; set; }
    }
}
