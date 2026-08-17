using System.Net;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.Enums;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Extensions;

public static class BaseResultExtensions
{
    private static readonly IReadOnlyDictionary<int, int> ErrorStatusCodeMap = new Dictionary<int, int>
    {
        // Data
        { (int)ErrorCodes.InvalidProperty, StatusCodes.Status400BadRequest },
        { (int)ErrorCodes.InvalidDataFormat, StatusCodes.Status400BadRequest },

        // User
        { (int)ErrorCodes.UserNotFound, StatusCodes.Status404NotFound },

        // Authorization
        { (int)ErrorCodes.OperationForbidden, StatusCodes.Status403Forbidden },

        // Question
        { (int)ErrorCodes.QuestionNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.QuestionsNotFound, StatusCodes.Status404NotFound },

        // Tags
        { (int)ErrorCodes.TagsNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.TagNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.TagAlreadyExists, StatusCodes.Status409Conflict },

        // Views
        { (int)ErrorCodes.ViewNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.ViewsNotFound, StatusCodes.Status404NotFound },

        // Votes
        { (int)ErrorCodes.VoteAlreadyGiven, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.VoteNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.VotesNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.CannotVoteForOwnPost, StatusCodes.Status403Forbidden },

        // Vote Types
        { (int)ErrorCodes.VoteTypeNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.VoteTypesNotFound, StatusCodes.Status404NotFound }
    };

    /// <summary>
    ///     Converts a BaseResult of type T into the corresponding ActionResult
    /// </summary>
    /// <param name="result"></param>
    /// <param name="successStatusCode"></param>
    /// <typeparam name="T">Type of BaseResult</typeparam>
    /// <returns></returns>
    public static ActionResult<BaseResult<T>> ToActionResult<T>(
        this BaseResult<T> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK) where T : class
    {
        if (result.IsSuccess) return new ObjectResult(result) { StatusCode = (int)successStatusCode };

        return new ObjectResult(result) { StatusCode = GetStatusCode(result.ErrorCode) };
    }

    /// <summary>
    ///     Converts a BaseResult into the corresponding ActionResult
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static ActionResult<BaseResult> ToActionResult(this BaseResult result)
    {
        if (result.IsSuccess) return new StatusCodeResult(StatusCodes.Status204NoContent);

        return new ObjectResult(result) { StatusCode = GetStatusCode(result.ErrorCode) };
    }

    private static int GetStatusCode(int? errorCode)
    {
        if (errorCode != null && ErrorStatusCodeMap.TryGetValue((int)errorCode, out var code)) return code;

        return StatusCodes.Status500InternalServerError;
    }
}
