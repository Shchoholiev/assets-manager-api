using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Infrastructure.Services;
using Moq;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace AssetsManagerApi.UnitTests;

[TestFixture]
public class NugetServiceTests
{
    private Mock<PackageMetadataResource> _metadataResourceMock;

    private NugetService _nugetService;

    [SetUp]
    public void SetUp()
    {
        _metadataResourceMock = new Mock<PackageMetadataResource>();

        typeof(NugetService)
            .GetField("_metadataResource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .SetValue(null, _metadataResourceMock.Object);

        _nugetService = new NugetService();
    }

    [Test]
    public async Task GetPackageLatestVersionAsync_ValidPackage_ReturnsLatestVersion()
    {
        // Arrange
        var packageName = "NUnit";
        var expectedVersion = "3.14.0";

        var mockMetadata = new Mock<IPackageSearchMetadata>();
        mockMetadata.Setup(m => m.Identity)
            .Returns(new NuGet.Packaging.Core.PackageIdentity(packageName, new NuGetVersion(expectedVersion)));

        var packageMetadata = new List<IPackageSearchMetadata> { mockMetadata.Object };

        _metadataResourceMock
            .Setup(m => m.GetMetadataAsync(
                packageName, 
                false, 
                false, 
                It.IsAny<SourceCacheContext>(), 
                It.IsAny<NuGet.Common.ILogger>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(packageMetadata);

        // Act
        var version = await _nugetService.GetPackageLatestVersionAsync(packageName, CancellationToken.None);

        // Assert
        Assert.That(version, Is.EqualTo(expectedVersion));
    }

    [Test]
    public void GetPackageLatestVersionAsync_InvalidPackage_ThrowsException()
    {
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _nugetService.GetPackageLatestVersionAsync("NotExistingPackage", CancellationToken.None));
    }
}
