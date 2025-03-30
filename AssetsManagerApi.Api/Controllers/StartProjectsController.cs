using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Compilation;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

/// <summary>
/// Controller for managing start projects.
/// </summary>
[Route("start-projects")]
public class StartProjectsController(
    IStartProjectsService startProjectsService
) : ApiController
{
    private readonly IStartProjectsService _startProjectsService = startProjectsService;
    
    /// <summary>
    /// Initializes a new start project and finds assets based on the provided project description.
    /// </summary>
    /// <param name="startProject">Prompt with project description</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A newly created start project with associated assets.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("")]
    [Produces("application/json")]
    public async Task<ActionResult<StartProjectDto>> CreateStartProjectAsync([FromBody] StartProjectCreateDto startProject, CancellationToken cancellationToken)
    {
        return await _startProjectsService.CreateStartProjectAsync(startProject, cancellationToken);
    }

    /// <summary>
    /// Creates a new code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileDto">The details of the code file to create.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created code file.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{startProjectId}/code-files")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeFileDto>> CreateCodeFileAsync(
        string startProjectId, // for future use
        [FromBody] CodeFileCreateDto codeFileDto,
        CancellationToken cancellationToken)
    {
        var createdCodeFile = await _startProjectsService.CreateCodeFileAsync(startProjectId, codeFileDto, cancellationToken);
        return Created("", createdCodeFile);
    }

    /// <summary>
    /// Updates an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to update.</param>
    /// <param name="codeFileDto">The updated code file details.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated code file.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPut("{startProjectId}/code-files/{codeFileId}")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeFileDto>> UpdateCodeFileAsync(
        string startProjectId, // for future use
        string codeFileId,
        [FromBody] CodeFileUpdateDto codeFileDto,
        CancellationToken cancellationToken)
    {
        var updatedCodeFile = await _startProjectsService.UpdateCodeFileAsync(startProjectId, codeFileId, codeFileDto, cancellationToken);
        return Ok(updatedCodeFile);
    }

    /// <summary>
    /// Deletes an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to delete.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if the deletion is successful.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpDelete("{startProjectId}/code-files/{codeFileId}")]
    [Produces("application/json")]
    public async Task<ActionResult> DeleteCodeFileAsync(
        string startProjectId, // for future use
        string codeFileId,
        CancellationToken cancellationToken)
    {
        await _startProjectsService.DeleteCodeFileAsync(startProjectId, codeFileId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Creates a new folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderDto">The details of the folder to create.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created folder.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{startProjectId}/folders")]
    [Produces("application/json")]
    public async Task<ActionResult<FolderDto>> CreateFolderAsync(
        string startProjectId, // for future use
        [FromBody] FolderCreateDto folderDto,
        CancellationToken cancellationToken)
    {
        var folder = await _startProjectsService.CreateFolderAsync(startProjectId, folderDto, cancellationToken);
        return Created("", folder);
    }

    /// <summary>
    /// Updates an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to update.</param>
    /// <param name="folderDto">The updated folder details.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated folder.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPut("{startProjectId}/folders/{folderId}")]
    [Produces("application/json")]
    public async Task<ActionResult<FolderDto>> UpdateFolderAsync(
        string startProjectId, // for future use
        string folderId,
        [FromBody] FolderUpdateDto folderDto,
        CancellationToken cancellationToken)
    {
        var folder = await _startProjectsService.UpdateFolderAsync(startProjectId, folderId, folderDto, cancellationToken);
        return Ok(folder);
    }

    /// <summary>
    /// Deletes an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>No content if the deletion is successful.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpDelete("{startProjectId}/folders/{folderId}")]
    [Produces("application/json")]
    public async Task<ActionResult> DeleteFolderAsync(
        string startProjectId, // for future use
        string folderId,
        CancellationToken cancellationToken)
    {
        await _startProjectsService.DeleteFolderAsync(startProjectId, folderId, cancellationToken);
        return NoContent();
    }


    /// <summary>
    /// Combines the code assets of a start project into a single project.
    /// </summary>
    /// <param name="id">The ID of the start project to combine.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Code Asset similar to Code Asset endpoints</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{id}/combine")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeAssetDto>> CombineStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        return await _startProjectsService.CombineStartProjectAsync(id, cancellationToken);
    }

    [Authorize(Roles = "Enterprise")]
    [HttpGet("{id}/combined-asset")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeAssetDto>> GetCombinedAssetAsync(string id, CancellationToken cancellationToken)
    {
        return await _startProjectsService.GetCombinedAssetAsync(id, cancellationToken);
    }

    /// <summary>
    /// Compiles a start project and returns the compilation result.
    /// </summary>
    /// <param name="id">The ID of the start project to compile.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The result of the compilation, including any errors if they occurred.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{id}/compile")]
    [Produces("application/json")]
    public async Task<ActionResult<CompilationResponse>> CompileStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        var compilationResult = await _startProjectsService.CompileStartProjectAsync(id, cancellationToken); 
        return compilationResult.Succeeded 
            ? Ok(compilationResult) 
            : BadRequest(compilationResult);
    }

    /// <summary>
    /// Downloads the start project as a zip file.
    /// </summary>
    /// <param name="id">The ID of the start project to download.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A zip file containing the project assets.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpGet("{id}/download")]
    [Produces("application/zip")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<FileContentResult> DownloadStartProjectZipAsync(string id, CancellationToken cancellationToken)
    {
        var (zipFileBytes, fileName) = await _startProjectsService.DownloadStartProjectZipAsync(id, cancellationToken);
        return File(zipFileBytes, "application/zip", fileName);
    }
}
