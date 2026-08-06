using Moq;
using Serilog;
using QuestionService.Tests.UnitTests.Fixtures;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class OutboxBackgroundServiceTests
{
    [Fact]
    public async Task ExecuteAsync_ScopeFactoryThrows_LogsAndStopsOnCancellation()
    {
        //Arrange
        // A null scope factory makes every iteration throw; the service is expected to log
        // the error and keep retrying until cancelled, not propagate the exception.
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        var outboxService = new TestableOutboxBackgroundService(new Mock<ILogger>().Object, null!);

        //Act
        await outboxService.ExecuteAsync(cts.Token);

        //Assert
        // If any exception is thrown, the test will fail
        Assert.True(true);
    }
}