using QuestionService.Tests.UnitTests.Configurations;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class OutboxBackgroundServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task ExecuteAsync_NullException_DoesNotThrow()
    {
        //Arrange
        var outboxService =
            new TestableOutboxBackgroundService(LoggerConfiguration.GetLogger(), null!); // passing null for exception

        //Act
        await outboxService.ExecuteAsync();

        //Assert
        // If any exception is thrown, the test will fail
        Assert.True(true);
    }
}