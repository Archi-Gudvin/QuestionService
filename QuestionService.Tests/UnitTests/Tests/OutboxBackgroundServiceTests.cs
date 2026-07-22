using QuestionService.Tests.UnitTests.Fixtures;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class OutboxBackgroundServiceTests
{
    [Fact]
    public async Task ExecuteAsync_NullException_DoesNotThrow()
    {
        //Arrange
        var outboxService =
            new TestableOutboxBackgroundService(LoggerFixture.GetLogger(), null!); // passing null for exception

        //Act
        await outboxService.ExecuteAsync();

        //Assert
        // If any exception is thrown, the test will fail
        Assert.True(true);
    }
}