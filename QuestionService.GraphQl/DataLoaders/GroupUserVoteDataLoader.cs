using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.GraphQl.DataLoaders.Base;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores votes by user ids
/// </summary>
public class GroupUserVoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedEntityDataLoader<Vote, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> FetchAsync(
        IServiceProvider serviceProvider, IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IGetVoteService>().GetUsersVotesAsync(keys, cancellationToken);
}
