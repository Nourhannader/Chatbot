using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Events
{
    public interface IEventHandler <in TEvent> where TEvent:IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
