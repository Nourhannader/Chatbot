using chatbot.Core.Interfaces.Outbox;

namespace chatbot.Api.BackgroundServices
{

    public sealed class OutboxBackgroundService(IServiceScopeFactory scopeFactory,ILogger<OutboxBackgroundService> logger): BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var scope =scopeFactory.CreateAsyncScope();

                    var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

                    await processor.ProcessAsync(stoppingToken);
                }
                catch (OperationCanceledException)when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError( ex, "Error while processing outbox messages.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5),stoppingToken);
            }
        }
    }

}
