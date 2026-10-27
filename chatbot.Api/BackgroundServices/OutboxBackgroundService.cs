using chatbot.Core.Interfaces.Outbox;

namespace chatbot.Api.BackgroundServices
{
    public sealed class OutboxBackgroundService(IServiceScopeFactory scopeFactory,ILogger<OutboxBackgroundService> logger): BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

        protected override async Task ExecuteAsync( CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

                    await processor.ProcessAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError( ex,"Error processing outbox.");
                }

                await Task.Delay(Interval,stoppingToken);
            }
        }
    }
}
