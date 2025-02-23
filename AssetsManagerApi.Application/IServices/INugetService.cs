namespace AssetsManagerApi.Application.IServices;

public interface INugetService
{
    public Task<string> GetPackageLatestVersionAsync(string packageName);
}
