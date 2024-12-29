using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AssetsManagerApi.Api.Controllers;

/// <summary>
/// Controller for managing start projects.
/// </summary>
[Route("start-projects")]
public class StartProjectsController : ApiController
{
    /// <summary>
    /// Initializes a new start project and finds assets based on the provided project description.
    /// </summary>
    /// <param name="startProject">Prompt with project description</param>
    /// <returns>A newly created start project with associated assets.</returns>
    [HttpPost("")]
    [Produces("application/json")]
    public async Task<ActionResult<StartProjectDto>> CreateStartProjectAsync([FromBody] StartProjectCreateDto startProject, CancellationToken cancellationToken)
    {
        var dummy = new StartProjectDto
        {
            Id = Guid.NewGuid().ToString(),
            CodeAssets = [
                new CodeAssetResult
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
                    RootFolderId = Guid.NewGuid().ToString(),
                    PrimaryCodeFile = new CodeFileDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Jwt.cs",
                        Language = Languages.csharp,
                        Text = @"
                            public string GenerateAccessToken(IEnumerable<Claim> claims)
                            {
                                var tokenOptions = GetTokenOptions(claims);
                                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                                this._logger.LogInformation(""Generated new access token."");

                                return tokenString;
                            }"
                    }
                }
            ]
        };

        return Ok(dummy);
    }

    /// <summary>
    /// Combines the code assets of a start project into a single project.
    /// </summary>
    /// <param name="id">The ID of the start project to combine.</param>
    /// <returns>List of code files similar.</returns>
    [HttpPost("{id}/combine")]
    [Produces("application/json")]
    public async Task<ActionResult> CombineStartProjectAsync(string id, CancellationToken cancellationToken)
    {
        return Ok();
    }

    /// <summary>
    /// Compiles a start project and returns the compilation result.
    /// </summary>
    /// <param name="id">The ID of the start project to compile.</param>
    /// <returns>The result of the compilation, including any errors if they occurred.</returns>
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
