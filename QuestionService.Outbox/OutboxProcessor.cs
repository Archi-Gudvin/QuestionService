using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuestionService.Outbox.Interfaces.Repository;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Outbox.Interfaces.TopicProducer;
using QuestionService.Outbox.Messages;
using Serilog;

namespace QuestionService.Outbox;

public class OutboxProcessor(
    IOutboxRepository outboxRepository,
    IServiceScopeFactory scopeFactory,
    ITopicProducerResolver producerResolver,
    ILogger logger)
    : IOutboxProcessor
{
    private static readonly TimeSpan[] RetryIntervals =
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(15),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(12),
        TimeSpan.FromHours(24)
    ];

    public async Task<int> ProcessOutboxMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var messages = await outboxRepository.GetUnprocessedAsync(batchSize, cancellationToken);

        var tasks = messages.Select(x => ProduceOutboxMessagesAsync(x, cancellationToken));

        var results = await Task.WhenAll(tasks);

        var count = results.Count(x => x);

        return count;
    }

    private async Task<bool> ProduceOutboxMessagesAsync(OutboxMessage message,
        CancellationToken cancellationToken = default)
    {
        // Create a new scope to get a new instance of IOutboxRepository for each thread
        await using var scope = scopeFactory.CreateAsyncScope();
        var currentOutbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

        try
        {
            var type = Assembly.GetExecutingAssembly().GetType(message.Type) ?? // FullName
                       Assembly.GetExecutingAssembly().GetTypes()
                           .First(x => x.Name == message.Type); // Name
            var content = JsonConvert.DeserializeObject(message.Content, type)!;

            var producer = producerResolver.GetProducerForType(scope.ServiceProvider, type);

            await producer.ProduceAsync(content, cancellationToken);

            await currentOutbox.MarkAsProcessedAsync(message.Id, CancellationToken.None);

            logger.Information("Produced message: {content}. Type: {type}", message.Content, type);

            return true;
        }
        catch (Exception e)
        {
            if (message.RetryCount >= RetryIntervals.Length)
                await currentOutbox.MarkAsDeadAsync(message.Id, e.Message, CancellationToken.None);
            else
                await currentOutbox.MarkAsFailedAsync(message.Id, e.Message, message.RetryCount + 1,
                    DateTime.UtcNow.Add(RetryIntervals[message.RetryCount]), CancellationToken.None);

            logger.Error(e, "Failed to produce message: {errorMessage}.", e.Message);

            return false;
        }
    }
}