using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.GraphQl.DataLoaders.Base;

namespace QuestionService.GraphQl.DataLoaders;

public class QuestionDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<Question, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<Question>> FetchAsync(IServiceProvider serviceProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IGetQuestionService>().GetByIdsAsync(keys, cancellationToken);

    protected override long GetId(Question entity) => entity.Id;
}
