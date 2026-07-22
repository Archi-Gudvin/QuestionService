using QuestionService.Outbox.Events;
using QuestionService.Outbox.TopicProducers;
using Xunit;
using QuestionService.Tests.Traits;

namespace QuestionService.Tests.UnitTests.Tests;

[UnitTest]
public class TopicProducerTests
{
    [Fact]
    public async Task ProduceAsync_NonBaseEventArgument_ThrowsArgumentException()
    {
        //Arrange
        var producer = new TopicProducer<BaseEvent>(null); //passing null for exception

        //Act
        var action = async () => await producer.ProduceAsync("notBaseEvent");

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
}