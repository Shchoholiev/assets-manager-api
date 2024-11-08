using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Paging;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AssetsManagerApi.Api.Controllers;

[Route("tags")]
public class TagsController(ITagsService tagsService) : ApiController
{
    private readonly ITagsService _tagsService = tagsService;

    [HttpGet]
    public async Task<ActionResult<PagedList<TagDto>>> GetTagsPage(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var tags = await this._tagsService.GetPopularTagsPage(pageNumber, pageSize, cancellationToken);
        return Ok(tags);
    }
}