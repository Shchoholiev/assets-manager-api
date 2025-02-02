using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

[Route("codeAssets")]
public class CodeAssetsController(ICodeAssetsService codeAssetsService) : ApiController
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    [HttpGet]
    public async Task<ActionResult<PagedList<CodeAssetDto>>> GetCodeAssetsPage([FromQuery] CodeAssetFilterModel filterModel, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.GetCodeAssetsPage(filterModel, pageNumber, pageSize, cancellationToken);
    }

    [HttpGet("{codeAssetId}")]
    public async Task<ActionResult<CodeAssetDto>> GetCodeAssetByIdPage(string codeAssetId, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.GetCodeAssetById(codeAssetId, cancellationToken);
    }
}
