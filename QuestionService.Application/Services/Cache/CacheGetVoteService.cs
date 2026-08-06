using QuestionService.Application.Enums;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetVoteService(IVoteCacheRepository cacheRepository, IGetVoteService inner) : IGetVoteService
{
    public Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Vote>> GetByDtosAsync(IReadOnlyCollection<VoteDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var dtosArray = dtos.ToArray();
        var votes = (await cacheRepository.GetByDtosAsync(dtosArray,
            async (dtosToFetch, ct) => (await inner.GetByDtosAsync(dtosToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (votes.Length == 0)
            return dtosArray.Length switch
            {
                <= 1 => CollectionResult<Vote>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound),
                > 1 => CollectionResult<Vote>.Failure(ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound)
            };

        return CollectionResult<Vote>.Success(votes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedVotes = (await cacheRepository.GetQuestionsVotesAsync(questionIds,
            async (idsToFetch, ct) => (await inner.GetQuestionsVotesAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedVotes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedVotes = (await cacheRepository.GetUsersVotesAsync(userIds,
            async (idsToFetch, ct) => (await inner.GetUsersVotesAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedVotes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetVoteTypesVotesAsync(
        IReadOnlyCollection<long> voteTypeIds, CancellationToken cancellationToken = default)
    {
        var groupedVotes = (await cacheRepository.GetVoteTypesVotesAsync(voteTypeIds,
            async (idsToFetch, ct) => (await inner.GetVoteTypesVotesAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (groupedVotes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }
}