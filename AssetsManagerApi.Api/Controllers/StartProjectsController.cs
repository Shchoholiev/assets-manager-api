using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
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
    /// <returns>The created code file.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("/{startProjectId}/code-files")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeFileDto>> CreateCodeFileAsync(
        string startProjectId,
        [FromBody] CodeFileDto codeFileDto,
        CancellationToken cancellationToken)
    {
        return Created("", codeFileDto);
    }

    /// <summary>
    /// Updates an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to update.</param>
    /// <param name="codeFileDto">The updated code file details.</param>
    /// <returns>The updated code file.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPut("/{startProjectId}/code-files/{codeFileId}")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeFileDto>> UpdateCodeFileAsync(
        string startProjectId,
        string codeFileId,
        [FromBody] CodeFileDto codeFileDto,
        CancellationToken cancellationToken)
    {
        return Ok(codeFileDto);
    }

    /// <summary>
    /// Deletes an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpDelete("/{startProjectId}/code-files/{codeFileId}")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeFileDto>> DeleteCodeFileAsync(
        string startProjectId,
        string codeFileId,
        CancellationToken cancellationToken)
    {
        return NoContent();
    }

    /// <summary>
    /// Creates a new folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderDto">The details of the folder to create.</param>
    /// <returns>The created folder.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("/{startProjectId}/folders")]
    [Produces("application/json")]
    public async Task<ActionResult<FolderDto>> CreateFolderAsync(
        string startProjectId,
        [FromBody] FolderDto folderDto,
        CancellationToken cancellationToken)
    {
        return Created("", folderDto);
    }

    /// <summary>
    /// Updates an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to update.</param>
    /// <param name="folderDto">The updated folder details.</param>
    /// <returns>The updated folder.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPut("/{startProjectId}/folders/{folderId}")]
    [Produces("application/json")]
    public async Task<ActionResult<FolderDto>> UpdateFolderAsync(
        string startProjectId,
        string folderId,
        [FromBody] FolderDto folderDto,
        CancellationToken cancellationToken)
    {
        return Ok(folderDto);
    }

    /// <summary>
    /// Deletes an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpDelete("/{startProjectId}/folders/{folderId}")]
    [Produces("application/json")]
    public async Task<ActionResult<FolderDto>> DeleteFolderAsync(
        string startProjectId,
        string folderId,
        CancellationToken cancellationToken)
    {
        return NoContent();
    }


    /// <summary>
    /// Combines the code assets of a start project into a single project.
    /// </summary>
    /// <param name="id">The ID of the start project to combine.</param>
    /// <returns>Code Asset similar to Code Asset endpoints</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{id}/combine")]
    [Produces("application/json")]
    public async Task<ActionResult<CodeAssetDto>> CombineStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        var primaryCodeFileId = Guid.NewGuid().ToString();
        var codeAsset = new CodeAssetDto
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Jwt Authentication",
            Description = "Authentication using Json Web Tokens",
            Tags =
                    [
                        new TagDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Jwt"
                        }
                    ],
            AssetType = AssetTypes.Public,
            Language = "CSharp",
            RootFolder =
                    new FolderDto
                    {
                        Id = "07efeec7-e902-4294-be0a-070f693472bb",
                        Name = "",
                        Type = FileType.Folder,
                        Items =
                        [
                            new CodeFileDto
                            {
                                Id = primaryCodeFileId,
                                Name = "Jwt.cs",
                                Language = "Csharp",
                                Text = @"
                                    public string GenerateAccessToken(IEnumerable<Claim> claims)
                                    {
                                        var tokenOptions = GetTokenOptions(claims);
                                        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                                        this._logger.LogInformation(""Generated new access token."");

                                        return tokenString;
                                    }",
                                Type = FileType.CodeFile
                            }
                        ]
                    },
            PrimaryCodeFile = new CodeFileDto
            {
                Id = primaryCodeFileId,
                Name = "Jwt.cs",
                Language = "Csharp",
                Text = @"
                    public string GenerateAccessToken(IEnumerable<Claim> claims)
                    {
                        var tokenOptions = GetTokenOptions(claims);
                        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                        this._logger.LogInformation(""Generated new access token."");

                        return tokenString;
                    }",
                Type = FileType.CodeFile
            }
        };
        return Ok(codeAsset);
    }

    /// <summary>
    /// Compiles a start project and returns the compilation result.
    /// </summary>
    /// <param name="id">The ID of the start project to compile.</param>
    /// <returns>The result of the compilation, including any errors if they occurred.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpPost("{id}/compile")]
    [Produces("application/json")]
    public async Task<ActionResult<CompilationResult>> CompileStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        var dummy = new CompilationResult
        {
            Error = null
        };
        return Ok(dummy);
    }

    /// <summary>
    /// Downloads the start project as a zip file.
    /// </summary>
    /// <param name="id">The ID of the start project to download.</param>
    /// <returns>A zip file containing the project assets.</returns>
    [Authorize(Roles = "Enterprise")]
    [HttpGet("{id}/download")]
    [Produces("application/zip")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<FileContentResult> DownloadStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        var zipFileBytes = CreateDummyZipFile();
        var fileName = "start-project.zip";

        return File(zipFileBytes, "application/zip", fileName);
    }

    private byte[] CreateDummyZipFile()
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            var demoFile = archive.CreateEntry("dummy.txt");
            using var entryStream = demoFile.Open();
            using var streamWriter = new StreamWriter(entryStream);
            streamWriter.Write("This is a dummy file for testing.");
        }

        return memoryStream.ToArray();
    }
}
