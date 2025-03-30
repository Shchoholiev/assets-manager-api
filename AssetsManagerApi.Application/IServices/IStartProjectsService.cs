using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Models.UpdateDto;

namespace AssetsManagerApi.Application.IServices;

public interface IStartProjectsService
{
    /// <summary>
    /// Creates a new start project based on the provided prompt.
    /// </summary>
    /// <param name="createDto">The DTO containing the start project creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created start project with its associated code assets.</returns>
    Task<StartProjectDto> CreateStartProjectAsync(StartProjectCreateDto createDto, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileDto">The DTO containing the code file details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created code file.</returns>
    Task<CodeFileDto> CreateCodeFileAsync(string startProjectId, CodeFileCreateDto codeFileDto, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to update.</param>
    /// <param name="codeFileDto">The updated code file details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated code file.</returns>
    Task<CodeFileDto> UpdateCodeFileAsync(string startProjectId, string codeFileId, CodeFileUpdateDto codeFileDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing code file for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="codeFileId">The identifier of the code file to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteCodeFileAsync(string startProjectId, string codeFileId, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderDto">The DTO containing the folder details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created folder.</returns>
    Task<FolderDto> CreateFolderAsync(string startProjectId, FolderCreateDto folderDto, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to update.</param>
    /// <param name="folderDto">The updated folder details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated folder.</returns>
    Task<FolderDto> UpdateFolderAsync(
        string startProjectId, 
        string folderId, 
        FolderUpdateDto folderDto, 
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing folder for the specified start project.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="folderId">The identifier of the folder to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteFolderAsync(string startProjectId, string folderId, CancellationToken cancellationToken);

    /// <summary>
    /// Combines the code assets of a start project into a single code asset.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project to combine.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The combined code asset.</returns>
    Task<CodeAssetDto> CombineStartProjectAsync(string startProjectId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns combined asset of a start project
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The combined code asset.</returns>
    Task<CodeAssetDto> GetCombinedAssetAsync(string startProjectId, CancellationToken cancellationToken);

    /// <summary>
    /// Compiles the start project and returns the compilation result.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project to compile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the compilation process.</returns>
    Task<CompilationResult> CompileStartProjectAsync(string startProjectId, CancellationToken cancellationToken);

    /// <summary>
    /// Downloads the start project as a zip file.
    /// </summary>
    /// <param name="startProjectId">The identifier of the start project to download.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The byte array representing the zip file content and name of zip.</returns>
    Task<(byte[] zipContent, string fileName)> DownloadStartProjectZipAsync(string startProjectId, CancellationToken cancellationToken);
}
