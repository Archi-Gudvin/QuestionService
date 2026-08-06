using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IQuestionVoteService
{
    /// <summary>
    ///     Increases question's reputation by 1
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<VoteQuestionDto>> UpvoteAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Decreases question's reputation by 1
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<VoteQuestionDto>> DownvoteAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes question's vote
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<VoteQuestionDto>> RemoveVoteAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default);
}