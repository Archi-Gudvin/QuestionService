using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestionService.Outbox.Interfaces.Service;
using Serilog;

namespace QuestionService.Outbox;

public class OutboxBackgroundService(ILogger logger, IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private const int OutboxProcessorFrequencyInSeconds = 15;
    private const int BatchSize = 50;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{ServiceName} is running.", nameof(OutboxBackgroundService));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                    var processed = await processor.ProcessOutboxMessagesAsync(BatchSize, stoppingToken);

                    if (processed > 0)
                        logger.Information("Processed {ProcessedMessagesCount} messages", processed);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    logger.Error(e, "An error occured while processing outbox messages: {ErrorMessage}", e.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequencyInSeconds), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.Warning("{ServiceName} is canceled.", nameof(OutboxBackgroundService));
        }
    }
}