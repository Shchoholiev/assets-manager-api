using Moq;
using Microsoft.Extensions.Logging;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Infrastructure.Services;
using AssetsManagerApi.Application.Models.Dto;

namespace AssetsManagerApi.UnitTests;

[TestFixture]
public class StartProjectsServiceTests
{
    private Mock<ICodeAssetsService> _codeAssetsServiceMock;
    private Mock<IGenerativeAiService> _generativeAiServiceMock;
    private Mock<IStartProjectsRepository> _startProjectsRepositoryMock;
    private Mock<ICodeFilesService> _codeFilesServiceMock;
    private Mock<IFoldersService> _foldersServiceMock;
    private Mock<ILogger<StartProjectsService>> _loggerMock;

    private Mock<INugetService> _nuggetServiceMock;

    private Mock<ICompilationService> _compilationService;

    private StartProjectsService _startProjectsService;

    [SetUp]
    public void SetUp()
    {
        _codeAssetsServiceMock = new Mock<ICodeAssetsService>();
        _generativeAiServiceMock = new Mock<IGenerativeAiService>();
        _startProjectsRepositoryMock = new Mock<IStartProjectsRepository>();
        _codeFilesServiceMock = new Mock<ICodeFilesService>();
        _foldersServiceMock = new Mock<IFoldersService>();
        _loggerMock = new Mock<ILogger<StartProjectsService>>();
        _nuggetServiceMock = new Mock<INugetService>();
        _compilationService = new Mock<ICompilationService>(); 

        _startProjectsService = new StartProjectsService(
            _codeAssetsServiceMock.Object,
            _generativeAiServiceMock.Object,
            _startProjectsRepositoryMock.Object,
            _codeFilesServiceMock.Object,
            _foldersServiceMock.Object,
            _loggerMock.Object,
            _nuggetServiceMock.Object,
            _compilationService.Object
        );
    }

    [Test]
    public async Task CreateCsprojAsync_ValidCodeFiles_ReturnsExpectedPackages()
    {
        // Arrange
        var codeFiles = new List<CodeFileDto>
            {
                new() { Text = "using System;\nusing System.IO;\nusing Newtonsoft.Json;" },
                new() { Text = "using System.Collections.Generic;\nusing Microsoft.Extensions.Logging;" },
                new() { Text = "using System.Linq;\nusing Moq;" }
            };

        var expectedPackages = new List<string>
            {
                "Newtonsoft.Json",
                "Microsoft.Extensions.Logging",
                "Moq"
            };

        // Setup NuGet service to return specific versions for each package.
        _nuggetServiceMock
            .Setup(s => s.GetPackageLatestVersionAsync("Newtonsoft.Json", It.IsAny<CancellationToken>()))
            .ReturnsAsync("13.0.1");
        _nuggetServiceMock
            .Setup(s => s.GetPackageLatestVersionAsync("Microsoft.Extensions.Logging", It.IsAny<CancellationToken>()))
            .ReturnsAsync("6.0.0");
        _nuggetServiceMock
            .Setup(s => s.GetPackageLatestVersionAsync("Moq", It.IsAny<CancellationToken>()))
            .ReturnsAsync("4.16.1");

        // Act
        var result = await _startProjectsService.CreateCsprojAsync(codeFiles, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        foreach (var package in expectedPackages)
        {
            StringAssert.Contains(package, result.Text);
        }
        StringAssert.Contains("Version=\"13.0.1\"", result.Text);
        StringAssert.Contains("Version=\"6.0.0\"", result.Text);
        StringAssert.Contains("Version=\"4.16.1\"", result.Text);
    }

    [Test]
    public async Task CreateCsprojAsync_DuplicateUsings_ReturnsPackageReferencesOnlyOnes()
    {
        // Arrange
        var codeFiles = new List<CodeFileDto>
            {
                new() { Text = "using Newtonsoft.Json;" },
                new() { Text = "using Newtonsoft.Json;" }
            };

        _nuggetServiceMock
            .Setup(s => s.GetPackageLatestVersionAsync("Newtonsoft.Json", It.IsAny<CancellationToken>()))
            .ReturnsAsync("13.0.1");

        // Act
        var result = await _startProjectsService.CreateCsprojAsync(codeFiles, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        var count = result.Text.Split("Newtonsoft.Json").Length - 1;
        Assert.That(count, Is.EqualTo(1), "Package reference should appear only once.");
        StringAssert.Contains("Version=\"13.0.1\"", result.Text);
    }

    [Test]
    public async Task CreateCsprojAsync_OnlySystemUsings_ReturnsNoPackages()
    {
        // Arrange
        var codeFiles = new List<CodeFileDto>
            {
                new() { Text = "using System;\nusing System.IO;\nusing System.Collections.Generic;" },
                new() { Text = "using System.Linq;" }
            };

        // Act
        var result = await _startProjectsService.CreateCsprojAsync(codeFiles, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Text, Does.Not.Contain("<PackageReference"));
    }
}
