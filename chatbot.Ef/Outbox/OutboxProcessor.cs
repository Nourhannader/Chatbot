using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Outbox;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Outbox
{
    public sealed class OutboxProcessor(IUnitOfWork unitOfWork,IEventPublisher eventPublisher,ILogger<OutboxProcessor> logger): IOutboxProcessor
    {
        private const int BatchSize = 50;
        private const int MaxRetries = 5;
        public async Task ProcessAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var messages = await unitOfWork.OutboxMessages.GetPendingAsync(BatchSize, now, cancellationToken);
                    

            foreach (var message in messages)
            {
                await ProcessMessageAsync( message, cancellationToken);
            }
        }

        private async Task ProcessMessageAsync(OutboxMessage message,CancellationToken cancellationToken)
        {
            try
            {
                message.Status = OutboxStatus.Processing;

                await unitOfWork.SaveChangesAsync(cancellationToken);

                var eventType = Type.GetType(message.EventType);

                if (eventType is null)
                {
                    throw new InvalidOperationException( $"Unknown event type: {message.EventType}");
                }

                var domainEvent =JsonSerializer.Deserialize(message.Payload,eventType)as IEvent;

                if (domainEvent is null)
                {
                    throw new InvalidOperationException("Could not deserialize event.");
                }

                await eventPublisher.PublishAsync( domainEvent, cancellationToken);

                message.Status =OutboxStatus.Processed;

                message.ProcessedAt =DateTime.UtcNow;

                message.LastError = null;

                message.NextRetryAt = null;

                await unitOfWork.SaveChangesAsync( cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleFailureAsync(message, ex,cancellationToken);
            }
        }

        private async Task HandleFailureAsync(OutboxMessage message, Exception exception,CancellationToken cancellationToken)
        {
            message.RetryCount++;

            message.LastError =exception.ToString();

            if (message.RetryCount >= MaxRetries)
            {
                message.Status =OutboxStatus.DeadLetter;

                message.NextRetryAt = null;
            }
            else
            {
                message.Status = OutboxStatus.Failed;

                message.NextRetryAt =CalculateNextRetry(message.RetryCount);
            }

            logger.LogError( exception, "Outbox event {EventId} failed. Retry {RetryCount}", message.EventId,message.RetryCount);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static DateTime CalculateNextRetry(int retryCount)
        {
            var delay =TimeSpan.FromSeconds( Math.Pow(2, retryCount));

            return DateTime.UtcNow.Add(delay);
        }
    }
}
