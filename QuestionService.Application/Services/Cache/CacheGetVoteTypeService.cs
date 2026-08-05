using QuestionService.Application.Enums;
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

    public async Task<CollectionResult<VoteType>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var voteTypes = (await cacheRepository.GetByIdsAsync(idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (voteTypes.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<VoteType>.Failure(ErrorMessage.VoteTypeNotFound,
                    (int)ErrorCodes.VoteTypeNotFound),
                > 1 => CollectionResult<VoteType>.Failure(ErrorMessage.VoteTypesNotFound,
                    (int)ErrorCodes.VoteTypesNotFound)
            };

        return CollectionResult<VoteType>.Success(voteTypes);
    }
}