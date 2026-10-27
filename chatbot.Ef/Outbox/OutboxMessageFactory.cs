using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Models;

namespace chatbot.Ef.Outbox
{
    public sealed class OutboxMessageFactory
    {
        private readonly IEventTypeRegistry _eventTypeRegistry;

        private readonly JsonSerializerOptions  _jsonOptions;

        public OutboxMessageFactory( IEventTypeRegistry eventTypeRegistry)
        {
            _eventTypeRegistry = eventTypeRegistry;

            _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        public  OutboxMessage Create<TEvent>(TEvent domainEvent)where TEvent : IEvent
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),

                EventId = domainEvent.EventId,

                EventType =_eventTypeRegistry.GetName<TEvent>(),

                Payload = JsonSerializer.Serialize( domainEvent,_jsonOptions),

                OccurredAt =domainEvent.OccurredAt,

                CreatedAt =DateTime.UtcNow,

                Status =OutboxStatus.Pending
            };
        }
    }
}
