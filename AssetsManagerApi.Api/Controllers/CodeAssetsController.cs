using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

[Route("codeAssets")]
public class CodeAssetsController(ICodeAssetsService codeAssetsService, IFoldersService foldersService, ICodeFilesService codeFilesService) : ApiController
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    private readonly IFoldersService _foldersService = foldersService;

    private readonly ICodeFilesService _codeFilesService = codeFilesService;

    [HttpGet]
    public async Task<ActionResult<PagedList<CodeAssetDto>>> GetCodeAssetsPageAsync([FromQuery] CodeAssetFilterModel filterModel, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.GetCodeAssetsPageAsync(filterModel, pageNumber, pageSize, cancellationToken);
    }

    [HttpGet("{codeAssetId}")]
    public async Task<ActionResult<CodeAssetDto>> GetCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.GetCodeAssetAsync(codeAssetId, cancellationToken);
    }

    [HttpDelete("{codeAssetId}")]
    public async Task<ActionResult<CodeAssetDto>> DeleteCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.DeleteCodeAssetAsync(codeAssetId, cancellationToken);
    }


    [HttpDelete("/codefiles/{codeFileId}")]
    public async Task<ActionResult<CodeFileDto>> DeleteCodeFileDtoAsync(string codeFileId, CancellationToken cancellationToken)
    {
        return await _codeFilesService.DeleteCodeFileAsync(codeFileId, cancellationToken);
    }


    [HttpDelete("/folders/{folderId}")]
    public async Task<ActionResult<FolderDto>> DeleteFolderAsync(string folderId, CancellationToken cancellationToken)
    {
        return await _foldersService.DeleteFolderAsync(folderId, cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<CodeAssetDto>> CreateCodeAssetAsync(CodeAssetCreateDto createDto, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.CreateCodeAssetAsync(createDto, cancellationToken);
    }

    [HttpPost("folders")]
    public async Task<ActionResult<FolderDto>> CreateFolderAsync([FromBody] FolderCreateDto createDto, CancellationToken cancellationToken)
    {
        return await _foldersService.CreateFolderAsync(createDto, cancellationToken);
    }

    [HttpPost("codefiles")]
    public async Task<ActionResult<CodeFileDto>> CreateCodeFileAsync([FromBody] CodeFileCreateDto createDto, CancellationToken cancellationToken)
    {
        return await _codeFilesService.CreateCodeFileAsync(createDto, cancellationToken);
    }


    [HttpPut]
    public async Task<ActionResult<CodeAssetDto>> UpdateCodeAssetAsync(CodeAssetDto dto, CancellationToken cancellationToken)
    {
    }

    [HttpPut("folders")]
    public async Task<ActionResult<FolderDto>> UpdateFolderAsync([FromBody] FolderDto dto, CancellationToken cancellationToken)
    {
    }

    [HttpPut("codefiles")]
    public async Task<ActionResult<CodeFileDto>> UpdateCodeFileAsync([FromBody] CodeFileDto dto, CancellationToken cancellationToken)
    {
        return await _codeFilesService.UpdateCodeFileAsync(dto, cancellationToken);
    }
}
