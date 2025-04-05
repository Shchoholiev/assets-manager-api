using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Application.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

/// <summary>
/// Controller for managing code assets, folders, and files.
/// </summary>
[Route("code-assets")]
public class CodeAssetsController(ICodeAssetsService codeAssetsService, IFoldersService foldersService, ICodeFilesService codeFilesService) : ApiController
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    private readonly IFoldersService _foldersService = foldersService;

    private readonly ICodeFilesService _codeFilesService = codeFilesService;

    /// <summary>
    /// Retrieves a paginated list of code assets with filtering.
    /// </summary>
    /// <param name="filterModel">Filter for searching code assets.</param>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>A paginated list of code assets.</returns>
    [HttpGet]
    public async Task<ActionResult<PagedList<CodeAssetDto>>> GetCodeAssetsPageAsync(
        [FromQuery] CodeAssetFilterModel filterModel,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await _codeAssetsService.GetCodeAssetsPageAsync(filterModel, pageNumber, pageSize, cancellationToken);
    }

    /// <summary>
    /// Retrieves a code asset by its ID.
    /// </summary>
    /// <param name="codeAssetId">The ID of the code asset.</param>
    /// <returns>The requested code asset.</returns>
    [HttpGet("{codeAssetId}")]
    public async Task<ActionResult<CodeAssetDto>> GetCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.GetCodeAssetAsync(codeAssetId, cancellationToken);
    }

    /// <summary>
    /// Deletes a code asset.
    /// </summary>
    /// <param name="codeAssetId">The ID of the code asset.</param>
    /// <returns>The deleted code asset.</returns>
    [Authorize]
    [HttpDelete("{codeAssetId}")]
    public async Task<ActionResult<CodeAssetDto>> DeleteCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.DeleteCodeAssetAsync(codeAssetId, cancellationToken);
    }

    /// <summary>
    /// Deletes a code file.
    /// </summary>
    /// <param name="codeFileId">The ID of the code file.</param>
    /// <returns>The deleted code file.</returns>
    [Authorize]
    [HttpDelete("codefiles/{codeFileId}")]
    public async Task<ActionResult<CodeFileDto>> DeleteCodeFileDtoAsync(string codeFileId, CancellationToken cancellationToken)
    {
        return await _codeFilesService.DeleteCodeFileAsync(codeFileId, cancellationToken);
    }

    /// <summary>
    /// Deletes a folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder.</param>
    /// <returns>The deleted folder.</returns>
    [Authorize]
    [HttpDelete("folders/{folderId}")]
    public async Task<ActionResult<FolderDto>> DeleteFolderAsync(string folderId, CancellationToken cancellationToken)
    {
        return await _foldersService.DeleteFolderAsync(folderId, cancellationToken);
    }

    /// <summary>
    /// Creates a new code asset.
    /// </summary>
    /// <param name="createDto">Data for creating a new code asset.</param>
    /// <returns>The created code asset.</returns>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CodeAssetDto>> CreateCodeAssetAsync(CodeAssetCreateDto createDto, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.CreateCodeAssetAsync(createDto, cancellationToken);
    }

    /// <summary>
    /// Creates a new folder.
    /// </summary>
    /// <param name="createDto">Data for creating a new folder.</param>
    /// <returns>The created folder.</returns>
    [Authorize]
    [HttpPost("folders")]
    public async Task<ActionResult<FolderDto>> CreateFolderAsync([FromBody] FolderCreateDto createDto, CancellationToken cancellationToken)
    {
        return await _foldersService.CreateFolderAsync(createDto, cancellationToken);
    }

    /// <summary>
    /// Creates a new code file.
    /// </summary>
    /// <param name="createDto">Data for creating a new code file.</param>
    /// <returns>The created code file.</returns>
    [Authorize]
    [HttpPost("codefiles")]
    public async Task<ActionResult<CodeFileDto>> CreateCodeFileAsync([FromBody] CodeFileCreateDto createDto, CancellationToken cancellationToken)
    {
        return await _codeFilesService.CreateCodeFileAsync(createDto, cancellationToken);
    }

    /// <summary>
    /// Updates an existing code asset.
    /// </summary>
    /// <param name="dto">Data for updating the code asset.</param>
    /// <returns>The updated code asset.</returns>
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<CodeAssetDto>> UpdateCodeAssetAsync([FromBody] CodeAssetUpdateDto dto, CancellationToken cancellationToken)
    {
        return await _codeAssetsService.UpdateCodeAssetAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Updates an existing folder.
    /// </summary>
    /// <param name="dto">Data for updating the folder.</param>
    /// <returns>The updated folder.</returns>
    [Authorize]
    [HttpPut("folders")]
    public async Task<ActionResult<FolderDto>> UpdateFolderAsync([FromBody] FolderUpdateDto dto, CancellationToken cancellationToken)
    {
        return await _foldersService.UpdateFolderAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Updates an existing code file.
    /// </summary>
    /// <param name="dto">Data for updating the code file.</param>
    /// <returns>The updated code file.</returns>
    [Authorize]
    [HttpPut("codefiles")]
    public async Task<ActionResult<CodeFileDto>> UpdateCodeFileAsync([FromBody] CodeFileUpdateDto dto, CancellationToken cancellationToken)
    {
        return await _codeFilesService.UpdateCodeFileAsync(dto, cancellationToken);
    }

     /// <summary>
    /// Downloads code asset as a zip file.
    /// </summary>
    /// <param name="id">The ID of the code asset to download.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A zip file containing the code assets.</returns>
    [Authorize]
    [HttpGet("{id}/download")]
    [Produces("application/zip")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<FileContentResult> DownloadCodeAssetZipAsync(string id, CancellationToken cancellationToken)
    {
        var (zipFileBytes, fileName) = await _codeAssetsService.GetCodeAssetAsZipAsync(id, cancellationToken);
        return File(zipFileBytes, "application/zip", fileName);
    }
}
