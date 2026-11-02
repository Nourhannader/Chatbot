using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Messages;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace chatbot.Ef.Events.Publishing
{
    public sealed class EventPublisher(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
    : IEventPublisher
    {
        public async Task PublishAsync( IEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handlerType =typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

            var enumerableType = typeof(IEnumerable<>) .MakeGenericType(handlerType);

            var handlers = serviceProvider.GetServices<IEventHandler<MessageCreatedEvent>>();

            if (handlers is null)
                return;

            foreach (var handler in handlers)
            {
                var handlerName = handler.GetType().FullName!;

                var alreadyProcessed = await unitOfWork.ProcessedEvents.ExistsAsync(domainEvent.EventId, handlerName, cancellationToken);
                        

                if (alreadyProcessed)
                    continue;

                var method = handlerType.GetMethod( "HandleAsync")!;

                var task =(Task)method.Invoke(
                        handler,
                        new object[]
                        {
                        domainEvent,
                        cancellationToken
                        })!;

                await task;

               await unitOfWork.ProcessedEvents.AddAsync(new ProcessedEvent
                    {
                        EventId = domainEvent.EventId,
                        HandlerName = handlerName
                    });

                await unitOfWork.SaveChangesAsync( cancellationToken);
            }
        }
    }
}
