using QuestionService.Application.Enums;
using QuestionService.Application.Extensions;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetVoteTypeService(IVoteTypeCacheRepository cacheRepository, IGetVoteTypeService inner)
    : IGetVoteTypeService
{
    public Task<QueryableResult<VoteType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<VoteType>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var voteTypes = (await cacheRepository.GetByIdsAsync(ids,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (voteTypes.Length == 0) return CollectionResult<VoteType>.VoteTypesNotFound(ids.Count);

        return CollectionResult<VoteType>.Success(voteTypes);
    }
}