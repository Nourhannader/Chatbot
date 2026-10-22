using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.MessageStatus
{
    public class MessageStatusSummaryDto
    {
        public Guid MessageId { get; set; }

        public int RecipientCount { get; set; }
        public int SentCount { get; set; }

        public int DeliveredCount { get; set; }

        public int ReadCount { get; set; }
    }
}
