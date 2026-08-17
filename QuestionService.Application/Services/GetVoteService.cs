using LinqKit;
using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Enums;
using QuestionService.Application.Extensions;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetVoteService(IBaseRepository<Vote> voteRepository) : IGetVoteService
{
    public Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var votes = voteRepository.GetAll();

        // Since there can be no votes, it is not an exception to have no votes 
        return Task.FromResult(QueryableResult<Vote>.Success(votes));
    }

    public async Task<CollectionResult<Vote>> GetByDtosAsync(IReadOnlyCollection<VoteDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var keys = dtos.ToArray();

        var predicate = PredicateBuilder.New<Vote>();
        predicate = keys.Aggregate(predicate,
            (current, local) =>
                current.Or(x => x.QuestionId == local.QuestionId && x.UserId == local.UserId));

        var votes = await voteRepository.GetAll()
            .AsExpandable()
            .Where(predicate)
            .ToArrayAsync(cancellationToken);

        if (votes.Length == 0) return CollectionResult<Vote>.VotesNotFound(dtos.Count);

        return CollectionResult<Vote>.Success(votes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default)
    {
        var votes = (await voteRepository.GetAll()
                .Where(x => questionIds.Contains(x.QuestionId))
                .GroupBy(x => x.QuestionId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Vote>>(x.Key, x.ToArray()))
            .ToArray();


        if (votes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(votes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IReadOnlyCollection<long> userIds, CancellationToken cancellationToken = default)
    {
        var votes = (await voteRepository.GetAll()
                .Where(x => userIds.Contains(x.UserId))
                .GroupBy(x => x.UserId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Vote>>(x.Key, x.ToArray()))
            .ToArray();

        if (votes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(votes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetVoteTypesVotesAsync(
        IReadOnlyCollection<long> voteTypeIds, CancellationToken cancellationToken = default)
    {
        var votes = (await voteRepository.GetAll()
                .Where(x => voteTypeIds.Contains(x.VoteTypeId))
                .GroupBy(x => x.VoteTypeId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Vote>>(x.Key, x.ToArray()))
            .ToArray();

        if (votes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(votes);
    }
}