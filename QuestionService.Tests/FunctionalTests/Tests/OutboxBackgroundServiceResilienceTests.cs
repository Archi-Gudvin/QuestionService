using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Events;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Outbox.Messages;
using QuestionService.Tests.FunctionalTests.Base.Exception.Outbox;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class OutboxBackgroundServiceResilienceTests(OutboxProcessorFailureFunctionalTestWebAppFactory factory)
    : OutboxProcessorFailureFunctionalTest(factory)
{
    [Fact]
    public async Task ExecuteBackgroundJob_FirstTickThrows_StillProcessesOnALaterTick()
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
        //First tick (~15s) throws via the mocked processor; second tick (~30s) must still run and process
        //the message, proving the background service survives a transient failure instead of exiting for good.
        await Task.Delay(TimeSpan.FromSeconds(35));

        //Assert
        var outboxMessages = await outboxRepository.GetAll().AsNoTracking().ToListAsync();
        Assert.True(outboxMessages.All(x => x.ProcessedAt != null));
    }
}
