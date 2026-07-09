using QuestionService.Outbox.Events;
using QuestionService.Outbox.TopicProducers;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class TopicProducerTests
{
    [Trait("Category", "Unit")]
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