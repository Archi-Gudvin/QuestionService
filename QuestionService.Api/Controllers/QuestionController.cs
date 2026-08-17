using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Api.Dtos;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     Question controller
/// </summary>
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public class QuestionController(IQuestionService questionService, IQuestionVoteService questionVoteService)
    : BaseController
{
    /// <summary>
    ///     Creates a question
    /// </summary>
    /// <response code="201">Question was created successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User or tags not found</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<QuestionDto>>> AskQuestionAsync(AskQuestionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await questionService.AskQuestionAsync(userId, dto, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    ///     Deletes a question
    /// </summary>
    /// <response code="200">Question was deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the owner of the question</response>
    /// <response code="404">User or question not found</response>
    [HttpDelete("{questionId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BaseResult<QuestionDto>>> DeleteQuestionAsync(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await questionService.DeleteQuestionAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Edits a question
    /// </summary>
    /// <response code="200">Question was edited successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the owner of the question</response>
    /// <response code="404">User, question or tags not found</response>
    [HttpPut("{questionId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BaseResult<QuestionDto>>> EditQuestionAsync(long questionId,
        RequestEditQuestionDto requestDto,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var dto = new EditQuestionDto(questionId, requestDto.Title, requestDto.Body, requestDto.TagNames);

        var result = await questionService.EditQuestionAsync(userId, dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Downvotes a question
    /// </summary>
    /// <response code="200">Vote was cast successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is voting on their own post or has insufficient reputation</response>
    /// <response code="404">User, question or vote type not found</response>
    /// <response code="409">User has already voted on this question</response>
    [HttpPatch("{questionId:long}/downvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> DownvoteQuestionAsync(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await questionVoteService.DownvoteAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Upvotes a question
    /// </summary>
    /// <response code="200">Vote was cast successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is voting on their own post or has insufficient reputation</response>
    /// <response code="404">User, question or vote type not found</response>
    /// <response code="409">User has already voted on this question</response>
    [HttpPatch("{questionId:long}/upvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> UpvoteQuestionAsync(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await questionVoteService.UpvoteAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Removes user's vote from a question
    /// </summary>
    /// <response code="200">Vote was removed successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User, question or vote not found</response>
    [HttpDelete("{questionId:long}/vote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> RemoveQuestionVoteAsync(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await questionVoteService.RemoveVoteAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }
}