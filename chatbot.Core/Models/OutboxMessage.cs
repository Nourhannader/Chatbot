using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;

namespace chatbot.Core.Models
{
    public class OutboxMessage : BaseEntity
    {
        public string EventType { get; set; } = null!;
        public Guid EventId { get; set; }

        public string Payload { get; set; } = null!;

        public DateTime OccurredAt { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        public int RetryCount { get; set; }

        public DateTime? NextRetryAt { get; set; }

        public string? LastError { get; set; }

        public OutboxStatus Status { get; set; }
            = OutboxStatus.Pending;
    }
}
