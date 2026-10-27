using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEvent domainEvent,CancellationToken cancellationToken = default);
    }
}
