using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces.Repository.Cache;

public interface IVoteCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of votes from the cache by their identifiers.
    ///     If any votes are missing, they are fetched using <paramref name="fetch"/> and cached.
    /// </summary>
    /// <param name="keys">The identifiers of the votes to retrieve.</param>
    /// <param name="fetch">A fallback delegate that fetches missing votes by their identifiers.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Vote}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<Vote>> GetByUserAndQuestionAsync(IEnumerable<VoteKey> keys,
        Func<IEnumerable<VoteKey>, CancellationToken, Task<IEnumerable<Vote>>> fetch,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves votes grouped by question identifiers from the cache.
    ///     If any question or votes are missing, the data is fetched using <paramref name="fetch"/> and cached accordingly.
    /// </summary>
    /// <param name="questionIds">The list of question IDs whose votes should be retrieved.</param>
    /// <param name="fetch">A fallback delegate that fetches missing votes grouped by question ids.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{Vote}" /> containing a lookup-like list of question Ids to the list of votes.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(IEnumerable<long> questionIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves votes grouped by user identifiers from the cache.
    ///     If any user or votes are missing, the data is fetched using <paramref name="fetch"/> and cached accordingly.
    /// </summary>
    /// <param name="userIds">The list of user IDs whose votes should be retrieved.</param>
    /// <param name="fetch">A fallback delegate that fetches missing votes grouped by user ids.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{Vote}" /> containing a lookup-like list of user Ids to the list of votes.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves votes grouped by vote type identifiers from the cache.
    ///     If any vote type or associated votes are missing, the data is fetched using <paramref name="fetch"/> and cached accordingly.
    /// </summary>
    /// <param name="voteTypeIds">The list of vote type IDs whose votes should be retrieved.</param>
    /// <param name="fetch">A fallback delegate that fetches missing votes grouped by vote type ids.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Vote}" /> containing a lookup-like list of vote type Ids to the list of votes.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetVoteTypesVotesAsync(IEnumerable<long> voteTypeIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default);
}