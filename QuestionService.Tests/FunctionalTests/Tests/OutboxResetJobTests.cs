using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.BackgroundJobs.Jobs;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Enums;
using QuestionService.Outbox.Messages;
using QuestionService.Tests.FunctionalTests.Base.Outboxless;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class OutboxResetJobTests : OutboxlessFunctionalTest
{
    public OutboxResetJobTests(OutboxlessFunctionalTestWebAppFactory factory) : base(factory)
    {
        using var scope = ServiceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();
        outboxRepository.GetAll().ExecuteDelete();
    }

    [Fact]
    public async Task RunAsync_OldAndRecentOutboxMessages_RemovesOldProcessedMessages()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();
        var outboxResetJob = ActivatorUtilities.CreateInstance<OutboxResetJob>(scope.ServiceProvider);

        var outboxMessages = new OutboxMessage[]
        {
            new()
            {
                ProcessedAt = DateTime.UtcNow.AddDays(-8), Type = "TestType1", Content = "TestContent1",
                Status = OutboxMessageStatus.Processed
            },
            new()
            {
                ProcessedAt = DateTime.UtcNow.AddDays(-8), Type = "TestType2", Content = "TestContent2", RetryCount = 3,
                NextRetryAt = DateTime.UtcNow.AddDays(-9), ErrorMessage = "TestError",
                Status = OutboxMessageStatus.Processed
            },
            new()
            {
                ProcessedAt = DateTime.UtcNow.AddDays(-1), Type = "TestType3", Content = "TestContent3",
                Status = OutboxMessageStatus.Processed
            },
            new()
            {
                ProcessedAt = null, Type = "TestType4", Content = "TestContent4", Status = OutboxMessageStatus.Pending
            },
            new()
            {
                ProcessedAt = null, Type = "TestType5", Content = "TestContent5",
                NextRetryAt = DateTime.UtcNow.AddDays(-1), RetryCount = 10, ErrorMessage = "TestError",
                Status = OutboxMessageStatus.Dead
            },
            new()
            {
                ProcessedAt = null, Type = "TestType6", Content = "TestContent6",
                NextRetryAt = DateTime.UtcNow.AddHours(-1), RetryCount = 2, ErrorMessage = "TestError",
                Status = OutboxMessageStatus.Failed
            }
        };
        await outboxRepository.CreateRangeAsync(outboxMessages);
        await outboxRepository.SaveChangesAsync();

        //Act
        await outboxResetJob.RunAsync();

        //Assert
        var count = await outboxRepository.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(4, count);
    }
}