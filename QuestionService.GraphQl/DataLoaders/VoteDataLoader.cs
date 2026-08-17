using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.GraphQl.DataLoaders.Base;

namespace QuestionService.GraphQl.DataLoaders;

public class VoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<Vote, VoteKey>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<Vote>> FetchAsync(IServiceProvider serviceProvider,
        IReadOnlyList<VoteKey> keys, CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IGetVoteService>().GetByUserAndQuestionAsync(keys, cancellationToken);

    protected override VoteKey GetId(Vote entity) => new(entity.QuestionId, entity.UserId);
}
