using QuestionService.Domain.Entities;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IGetQuestionService : IGetService<Question>
{
    /// <summary>
    ///     Gets questions with tags by their names 
    /// </summary>
    /// <param name="tagIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IReadOnlyCollection<long> tagIds, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets questions of users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IReadOnlyCollection<long> userIds, CancellationToken cancellationToken = default);
}