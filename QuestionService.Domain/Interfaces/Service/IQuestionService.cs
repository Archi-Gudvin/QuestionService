using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IQuestionService
{
    /// <summary>
    ///     Creates a question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> AskQuestionAsync(long initiatorId, AskQuestionDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Edits question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> EditQuestionAsync(long initiatorId, EditQuestionDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes the question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> DeleteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default);
}