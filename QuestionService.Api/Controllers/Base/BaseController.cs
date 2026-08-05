using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.Enums;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers.Base;

/// <inheritdoc />
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ApiController]
public class BaseController : ControllerBase
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
    ///     Handles the BaseResult of type T and returns the corresponding ActionResult 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="successStatusCode"></param>
    /// <typeparam name="T">Type of BaseResult</typeparam>
    /// <returns></returns>
    protected ActionResult<BaseResult<T>> HandleBaseResult<T>(
        BaseResult<T> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK) where T : class
    {
        var statusCode = GetStatusCode(result.IsSuccess, result.ErrorCode, (int)successStatusCode);
        return StatusCode(statusCode, result);
    }


    /// <summary>
    ///     Handles the BaseResult and returns the corresponding ActionResult 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    protected ActionResult<BaseResult> HandleBaseResult(BaseResult result)
    {
        var statusCode = GetStatusCode(result.IsSuccess, result.ErrorCode, StatusCodes.Status204NoContent);
        return StatusCode(statusCode, result);
    }

    private static int GetStatusCode(bool isSuccess, int? errorCode, int successStatusCode)
    {
        const int defaultCode = StatusCodes.Status400BadRequest;

        if (isSuccess) return successStatusCode;
        if (errorCode == null || !ErrorStatusCodeMap.TryGetValue((int)errorCode, out var code)) return defaultCode;
        return code;
    }
}