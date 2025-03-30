using AssetsManagerApi.Application.Models.Dto;

namespace AssetsManagerApi.Application.IServices;

public interface IGenerativeAiService
{
    /// <summary>
    /// Select relevant code assets based on the project description.
    /// </summary>
    /// <param name="projectDescription"></param>
    /// <param name="assets"></param>
    /// <returns>List of Selected Code Assets</returns>
    Task<List<CodeAssetDto>> SelectRelevantCodeAssets(
        string projectDescription, 
        IEnumerable<CodeAssetDto> assets, 
        CancellationToken cancellationToken);
}
