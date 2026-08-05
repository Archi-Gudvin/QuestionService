using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Api.Dtos;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     Question controller
/// </summary>
[Authorize]
public class QuestionController(IQuestionService questionService, IQuestionVoteService questionVoteService)
    : BaseController
{
    /// <summary>
    ///     Creates a question
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to ask a question:
    ///
    ///     POST
    ///     {
    ///         "title":"string",
    ///         "body":"string",
    ///         "tagNames":[
    ///            "string"
    ///          ]
    ///     }
    /// </remarks>
    /// <response code="201">Question was created successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User or tags not found</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<QuestionDto>>> AskQuestion(AskQuestionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await questionService.AskQuestionAsync(userId, dto, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    ///     Deletes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to delete a question:
    ///
    ///     DELETE {questionId}
    /// </remarks>
    /// <response code="200">Question was deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the owner of the question</response>
    /// <response code="404">User or question not found</response>
    [HttpDelete("{questionId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<QuestionDto>>> DeleteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.DeleteQuestionAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Edits a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to edit a question:
    ///
    ///     PUT
    ///     {
    ///         "title":"string",
    ///         "body":"string",
    ///         "tagNames":[
    ///            "string"
    ///          ]
    ///     }
    /// </remarks>
    /// <response code="200">Question was edited successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the owner of the question</response>
    /// <response code="404">User, question or tags not found</response>
    [HttpPut("{questionId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<QuestionDto>>> EditQuestion(long questionId,
        RequestEditQuestionDto requestDto,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var dto = new EditQuestionDto(questionId, requestDto.Title, requestDto.Body, requestDto.TagNames);

        var result = await questionService.EditQuestionAsync(userId, dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Downvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to downvote a question:
    ///
    ///     PATCH {questionId}/downvote
    /// </remarks>
    /// <response code="200">Vote was cast successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is voting on their own post or has insufficient reputation</response>
    /// <response code="404">User, question or vote type not found</response>
    /// <response code="409">User has already voted on this question</response>
    [HttpPatch("{questionId:long}/downvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> DownvoteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionVoteService.DownvoteAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Upvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to upvote a question:
    ///
    ///     PATCH {questionId}/upvote
    /// </remarks>
    /// <response code="200">Vote was cast successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is voting on their own post or has insufficient reputation</response>
    /// <response code="404">User, question or vote type not found</response>
    /// <response code="409">User has already voted on this question</response>
    [HttpPatch("{questionId:long}/upvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> UpvoteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionVoteService.UpvoteAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Removes user's vote from a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to remove a vote from a question:
    ///
    ///     DELETE {questionId}/vote
    /// </remarks>
    /// <response code="200">Vote was removed successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User, question or vote not found</response>
    [HttpDelete("{questionId:long}/vote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> RemoveQuestionVote(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionVoteService.RemoveVoteAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }
}