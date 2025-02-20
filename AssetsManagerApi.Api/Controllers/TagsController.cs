using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AssetsManagerApi.Api.Controllers;

/// <summary>
/// Controller for managing tags.
/// </summary>
[Route("tags")]
public class TagsController(ITagsService tagsService) : ApiController
{
    private readonly ITagsService _tagsService = tagsService;

    /// <summary>
    /// Retrieves a paginated list of popular tags with optional search string.
    /// </summary>
    /// <param name="searchString">Optional search string to filter tags.</param>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>A paginated list of tags.</returns>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<TagDto>>> GetTagsPage(
        [FromQuery] string? searchString,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var tags = await this._tagsService.GetPopularTagsPage(searchString, pageNumber, pageSize, cancellationToken);
        return Ok(tags);
    }
}
