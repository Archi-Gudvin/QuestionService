using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.GraphQl.DataLoaders.Base;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.DataLoaders;

public class TagDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<Tag, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<Tag>> FetchAsync(IServiceProvider serviceProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IGetTagService>().GetByIdsAsync(keys, cancellationToken);

    protected override long GetId(Tag entity) => entity.Id;
}
