using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     Tag controller
/// </summary>
[Authorize(Roles = $"{nameof(Roles.Moderator)},{nameof(Roles.Admin)}")]
public class TagController(ITagService tagService) : BaseController
{
    /// <summary>
    ///     Creates a tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request to create a tag:
    ///
    ///     POST
    ///     {
    ///         "name":"string",
    ///         "description":"string"
    ///     }
    /// </remarks>
    /// <returns></returns>
    /// <response code="201">Tag was created successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Moderator or Admin role</response>
    /// <response code="409">Tag with this name already exists</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<TagDto>>> CreateTagAsync([FromBody] CreateTagDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tagService.CreateTagAsync(dto, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    ///     Updates a tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request to update a tag:
    ///
    ///     PUT
    ///     {
    ///         "id":1,
    ///         "name":"string",
    ///         "description":"string"
    ///     }
    /// </remarks>
    /// <returns></returns>
    /// <response code="200">Tag was updated successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Moderator or Admin role</response>
    /// <response code="404">Tag not found</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<TagDto>>> UpdateTagAsync([FromBody] TagDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tagService.UpdateTagAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Deletes a tag
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request to delete a tag:
    ///
    ///     DELETE {id:long}
    /// </remarks>
    /// <returns></returns>
    /// <response code="200">Tag was deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Moderator or Admin role</response>
    /// <response code="404">Tag not found</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<TagDto>>> DeleteTagAsync(long id,
        CancellationToken cancellationToken)
    {
        var result = await tagService.DeleteTagAsync(id, cancellationToken);

        return HandleBaseResult(result);
    }
}