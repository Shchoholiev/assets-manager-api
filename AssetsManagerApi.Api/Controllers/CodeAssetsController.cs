using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Paging;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AssetsManagerApi.Api.Controllers;

[Route("codeAssets")]
public class CodeAssetsController(ICodeAssetsService codeAssetsService) : ApiController
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    public async Task<ActionResult<PagedList<CodeAssetDto>>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.GetCodeAssetsPage(pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }

    [HttpGet("codeAssets")]
    public async Task<ActionResult<PagedList<CodeAssetDto>>> GetCodeAssetsPage(int pageNumber, int pageSize, Expression<Func<CodeAssetDto, bool>> predicate, CancellationToken cancellationToken)
    {
        var codeAssets = await this._codeAssetsService.GetCodeAssetsPage(pageNumber, pageSize, cancellationToken);
        return Ok(codeAssets);
    }
}
