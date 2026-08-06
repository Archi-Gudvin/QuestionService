using QuestionService.Domain.Entities;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IGetViewService : IGetService<View>
{
    /// <summary>
    ///     Gets questions of users by their ids 
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(
        IReadOnlyCollection<long> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets views of questions by their ids
    /// </summary>
    /// <param name="questionIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IReadOnlyCollection<long> questionIds, CancellationToken cancellationToken = default);
}