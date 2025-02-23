using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Application.IServices;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace AssetsManagerApi.Infrastructure.Services;

public class NugetService : INugetService
{
    private readonly string NuGetUrl = "https://api.nuget.org/v3/index.json";

    private static PackageMetadataResource _metadataResource;

    public NugetService()
    {
        InitializeNuGetResourceAsync().Wait();
    }

    private async Task InitializeNuGetResourceAsync()
    {
        if (_metadataResource == null)
        {
            var repository = Repository.Factory.GetCoreV3(NuGetUrl);
            _metadataResource = await repository.GetResourceAsync<PackageMetadataResource>();
        }
    }

    public async Task<string> GetPackageLatestVersionAsync(string packageName)
    {
        var packageMetadata = await _metadataResource.GetMetadataAsync(
            packageName, 
            includePrerelease: false, 
            includeUnlisted: false, 
            new SourceCacheContext(), 
            NuGet.Common.NullLogger.Instance, 
            CancellationToken.None);
        
        if (packageMetadata == null || !packageMetadata.Any())
        {
            throw new EntityNotFoundException($"Package '{packageName}' not found.");
        }
        
        var latestVersion = packageMetadata
            .Select(m => m.Identity.Version)
            .OrderByDescending(v => v)
            .FirstOrDefault();

        return latestVersion?.ToString() ?? throw new EntityNotFoundException($"No valid versions found for package '{packageName}'.");
    }
}