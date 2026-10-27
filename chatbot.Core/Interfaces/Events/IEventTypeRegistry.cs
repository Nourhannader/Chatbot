using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Events
{
    public interface IEventTypeRegistry
    {
        string GetName(Type eventType);

        string GetName<TEvent>()  where TEvent : IEvent;

        Type GetType(string eventName);
    }
}
