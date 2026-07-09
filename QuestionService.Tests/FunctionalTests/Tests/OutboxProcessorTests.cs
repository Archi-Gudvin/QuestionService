using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Enums;
using QuestionService.Outbox.Events;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Tests.FunctionalTests.Base.Exception;
using Xunit;
using OutboxMessage = QuestionService.Outbox.Messages.OutboxMessage;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class OutboxProcessorTests(ExceptionFunctionalTestWebAppFactory factory) : ExceptionFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ProcessOutboxMessages_EventTypeWithoutRegisteredProducer_ReturnsFailedMessages()
    {
        //Arrange
        const long userId = 1;

        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();

        await outboxService.AddToOutboxAsync(new BaseEvent
        {
            EventId = Guid.NewGuid(),
            EventType = nameof(BaseEventType.EntityUpvoted),
            AuthorId = userId
        });

        //Act
        await Task.Delay(TimeSpan.FromSeconds(20)); //Waiting for OutboxBackgroundService to execute the job

        //Assert
        var unprocessedMessages =
            await outboxRepository.GetAll().Where(x => x.ProcessedAt == null).AsNoTracking().ToListAsync();
        Assert.True(unprocessedMessages.All(x => x.ErrorMessage != null));
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ProcessOutboxMessages_MaxRetriesReached_ReturnsDeadStatus()
    {
        // Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();
        var outboxMessage = new OutboxMessage
        {
            Content = "TestContent",
            Type = "TestType",
            ErrorMessage = "TestError",
            NextRetryAt = DateTime.UtcNow.AddMinutes(-1),
            RetryCount = 10,
            Status = OutboxMessageStatus.Failed
        };
        await outboxRepository.CreateAsync(outboxMessage);
        await outboxRepository.SaveChangesAsync();

        // Act
        await Task.Delay(TimeSpan.FromSeconds(20)); //Waiting for OutboxBackgroundService to execute the job

        // Assert
        var processedMessage = await outboxRepository.GetAll().AsNoTracking().FirstAsync(x => x.Id == outboxMessage.Id);
        Assert.Equal(OutboxMessageStatus.Dead, processedMessage.Status);
    }
}