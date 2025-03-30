using AssetsManagerApi.Application.Models.Compilation;

namespace AssetsManagerApi.Application.IServices;

public interface ICompilationService
{
    Task<CompilationResponse> CompileDotNetProjectAsync(byte[] projectZip, CancellationToken cancellationToken);
}
