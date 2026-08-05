using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.GraphQl.DataLoaders.Base;

namespace QuestionService.GraphQl.DataLoaders;

public class ViewDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<View, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<View>> FetchAsync(IServiceProvider serviceProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IGetViewService>().GetByIdsAsync(keys, cancellationToken);

    protected override long GetId(View entity) => entity.Id;
}
