using Microsoft.Extensions.DependencyInjection;
using QuestionService.Outbox.Interfaces.TopicProducer;

namespace QuestionService.Outbox.TopicProducers;

public class TopicProducerResolver : ITopicProducerResolver
{
    public ITopicProducer GetProducerForType(IServiceProvider serviceProvider, Type messageType)
    {
        var producers = serviceProvider.GetRequiredService<IEnumerable<ITopicProducer>>();

        return producers.FirstOrDefault(x => x.CanProduce(messageType)) ??
               throw new InvalidOperationException($"No producer found for type {messageType}.");
    }
}