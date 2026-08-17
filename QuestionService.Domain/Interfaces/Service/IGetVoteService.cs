using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IGetVoteService
{
    /// <summary>
    ///     Gets all votes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets vote of questions by pairs of question id and user id
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<Vote>> GetByUserAndQuestionAsync(IReadOnlyCollection<VoteKey> keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets votes of questions by their ids
    /// </summary>
    /// <param name="questionIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets votes of users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IReadOnlyCollection<long> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets votes of vote types by their ids
    /// </summary>
    /// <param name="voteTypeIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetVoteTypesVotesAsync(
        IReadOnlyCollection<long> voteTypeIds, CancellationToken cancellationToken = default);
}