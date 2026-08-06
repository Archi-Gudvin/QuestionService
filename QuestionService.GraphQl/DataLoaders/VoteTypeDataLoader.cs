using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.GraphQl.DataLoaders.Base;

namespace QuestionService.GraphQl.DataLoaders;

public class VoteTypeDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<VoteType, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<VoteType>> FetchAsync(IServiceProvider serviceProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IGetVoteTypeService>().GetByIdsAsync(keys, cancellationToken);

    protected override long GetId(VoteType entity) => entity.Id;
}
