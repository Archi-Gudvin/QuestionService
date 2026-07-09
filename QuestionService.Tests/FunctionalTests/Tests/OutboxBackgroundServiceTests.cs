using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Events;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Outbox.Messages;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class OutboxBackgroundServiceTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ExecuteBackgroundJob_PendingOutboxMessage_ProcessesMessage()
    {
        //Arrange
        const long userId = 1;

        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();

        await outboxService.AddToOutboxAsync(new BaseEvent
        {
            EventId = Guid.NewGuid(),
            EventType = nameof(BaseEventType.EntityUpvoted),
            AuthorId = userId
        });

        //Act
        await Task.Delay(TimeSpan.FromSeconds(20)); //Waiting for OutboxBackgroundService to execute the job

        //Assert
        var outboxMessages = await outboxRepository.GetAll().AsNoTracking().ToListAsync();
        Assert.True(outboxMessages.All(x => x.ProcessedAt != null));
    }
}