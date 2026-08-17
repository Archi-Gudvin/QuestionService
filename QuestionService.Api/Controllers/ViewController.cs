using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     View controller
/// </summary>
public class ViewController(IViewService viewService) : BaseController
{
    private const string FingerprintHeaderName = "X-Fingerprint";

    /// <summary>
    ///     Increments views of a question by its id
    /// </summary>
    /// <response code="204">Views were incremented successfully</response>
    /// <response code="400">Invalid data format</response>
    [HttpPost("{questionId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult>> IncrementViewsAsync(long questionId,
        CancellationToken cancellationToken,
        [FromHeader(Name = FingerprintHeaderName)]
        string? fingerprint = null)
    {
        if (!TryGetUserIp(out var userIp)) return BadRequest("IP Address is not provided");
        if (fingerprint == null) return BadRequest("Fingerprint is not provided");

        var userId = GetUserIdIfExists();

        var dto = new IncrementViewsDto(questionId, userId, userIp, fingerprint);

        var result = await viewService.IncrementViewsAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    private long? GetUserIdIfExists()
    {
        return User.TryGetUserId(out var userId) ? userId : null;
    }

    /// <summary>
    /// IMPORTANT: User IP depends on KnownProxies in config.
    /// Set KnownProxies to access real user IP.
    /// Known proxies are known reverse proxies or load balancers.
    /// Known reverse proxies or load balancers are assumed to remove custom X-Forwarded-For headers before sending request to this server.
    /// </summary>
    /// <param name="userIp"></param>
    /// <returns></returns>
    private bool TryGetUserIp([MaybeNullWhen(false)] out string userIp)
    {
        userIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        return !string.IsNullOrEmpty(userIp);
    }
}