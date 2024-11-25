using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

[Route("codeAssets")]
public class CodeAssetsController(ICodeAssetsService codeAssetsService) : ApiController
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    [HttpGet]
    public async Task<ActionResult<PagedList<CodeAssetResult>>> GetCodeAssetsPage([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.GetCodeAssetsPage(pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }

    [HttpGet("{codeAssetId}")]
    public async Task<ActionResult<CodeAssetResult>> GetCodeAssetByIdPage(string codeAssetId, CancellationToken cancellationToken)
    {
        var codeAsset = await this._codeAssetsService.GetCodeAssetById(codeAssetId, cancellationToken);
        return Ok(codeAsset);
    }

    [HttpGet("byUser")]
    public async Task<ActionResult<PagedList<CodeAssetResult>>> GetUsersCodeAssetsPage([FromQuery] string userId, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.GetUsersCodeAssetsPage(userId, pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }

    [HttpGet("byTags")]
    public async Task<ActionResult<PagedList<CodeAssetResult>>> GetCodeAssetsByTagsPage([FromQuery] List<string> tagIds, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.GetCodeAssetsByTagsPage(tagIds, pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedList<CodeAssetResult>>> SearchCodeAssetsPage([FromQuery] string input, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.SearchCodeAssetsPage(input, pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }
}
