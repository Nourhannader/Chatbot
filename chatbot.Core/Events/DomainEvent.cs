using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Events
{
    public abstract record DomainEvent : IEvent
    {
        public Guid EventId { get; init; }
            = Guid.NewGuid();

        public DateTime OccurredAt { get; init; }
            = DateTime.UtcNow;
    }
}
