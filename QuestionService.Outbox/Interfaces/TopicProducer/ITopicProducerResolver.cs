namespace QuestionService.Outbox.Interfaces.TopicProducer;

public interface ITopicProducerResolver
{
    ITopicProducer GetProducerForType(IServiceProvider serviceProvider, Type messageType);
}