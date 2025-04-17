using Moq;
using Microsoft.Extensions.Logging;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Infrastructure.Services;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Domain.Enums;
using System.Text.Json;
using AssetsManagerApi.Application.Models.UpdateDto;

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
    public async Task CombineStartProject_TwoCodeAssets_ReturnsCombinedCodeAsset()
    {
        // Arrange
        string startProjectId = "123";

        // Create a fake start project with two asset IDs.
        var startProject = new StartProject
        {
            Id = startProjectId,
            CodeAssetsIds = ["asset1", "asset2"]
        };

        _startProjectsRepositoryMock
            .Setup(repo => repo.GetOneAsync(startProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(startProject);

        // Set up the CreateCodeAssetAsync to return the combined asset.
        var combinedAssetDto = new CodeAssetDto { Id = "combinedAssetId", Name = "Start Project" };
        _codeAssetsServiceMock
            .Setup(svc => svc.CreateCodeAssetAsync(It.IsAny<CodeAssetCreateDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(combinedAssetDto);

        // Set up two dummy code assets to be combined.
        var asset1 = new CodeAssetDto
        {
            Id = "asset1",
            Tags = new List<TagDto>
            {
                new TagDto { Id = "tag3", Name = "tag1" },
                new TagDto { Id = "tag4", Name = "UI" }
            },
            RootFolder = new FolderDto
            {
                Id = "folder1",
                Name = "ProjectOne",
                Type = FileType.Folder,
                Items = new List<FileSystemNodeDto>
                {
                    // A code file directly under the root folder.
                    new CodeFileDto
                    {
                        Id = "code1",
                        Name = "File1.cs",
                        Type = FileType.CodeFile,
                        Language = "Csharp",
                        Text = "public class Foo { }"
                    },
                    // A nested folder (e.g., Controllers) with an additional code file.
                    new FolderDto
                    {
                        Id = "subfolder1",
                        Name = "Controllers",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>
                        {
                            new CodeFileDto
                            {
                                Id = "codeSub1",
                                Name = "HomeController.cs",
                                Type = FileType.CodeFile,
                                Language = "Csharp",
                                Text = "public class HomeController { }"
                            }
                        }
                    },
                    new CodeFileDto
                    {
                        Id = "codeProgram",
                        Name = "Program.cs",
                        Type = FileType.CodeFile,
                        Language = "Csharp",
                        Text = "public class Program { }"
                    },
                }
            }
        };

        var asset2 = new CodeAssetDto
        {
            Id = "asset2",
            Tags = new List<TagDto>
            {
                new TagDto { Id = "tag1", Name = "tag2" },
                new TagDto { Id = "tag2", Name = "API" }
            },
            RootFolder = new FolderDto
            {
                Id = "folder2",
                Name = "ProjectTwo",
                Type = FileType.Folder,
                Items = new List<FileSystemNodeDto>
                {
                    // A code file directly under the root folder.
                    new CodeFileDto
                    {
                        Id = "code2",
                        Name = "File2.cs",
                        Type = FileType.CodeFile,
                        Language = "Csharp",
                        Text = "public class Bar { }"
                    },
                    // A nested folder (e.g., Services) with two code files.
                    new FolderDto
                    {
                        Id = "subfolder2",
                        Name = "Services",
                        Type = FileType.Folder,
                        Items = new List<FileSystemNodeDto>
                        {
                            new CodeFileDto
                            {
                                Id = "codeSub2",
                                Name = "IMyService.cs",
                                Type = FileType.CodeFile,
                                Language = "Csharp",
                                Text = "public interface IMyService { }"
                            },
                            new CodeFileDto
                            {
                                Id = "codeSub3",
                                Name = "MyService.cs",
                                Type = FileType.CodeFile,
                                Language = "Csharp",
                                Text = "public class MyService : IMyService { }"
                            }
                        }
                    },
                    new CodeFileDto
                    {
                        Id = "codeStartup",
                        Name = "Startup.cs",
                        Type = FileType.CodeFile,
                        Language = "Csharp",
                        Text = "public class Startup { }"
                    },
                }
            }
        };


        _codeAssetsServiceMock
            .Setup(svc => svc.GetCodeAssetAsync("asset1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset1);

        _codeAssetsServiceMock
            .Setup(svc => svc.GetCodeAssetAsync("asset2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset2);


        var mergedFolder = new FolderDto();
        _foldersServiceMock
            .Setup(svc => svc.SaveFolderHierarchyAsync(It.IsAny<FolderDto>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FolderDto folder, string parentId, CancellationToken token) =>
            {
                mergedFolder = folder;
                return mergedFolder;
            });
            
        _codeAssetsServiceMock
            .Setup(svc => svc.UpdateCodeAssetAsync(It.IsAny<CodeAssetUpdateDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CodeAssetUpdateDto asset, CancellationToken token) =>
            {
                var result = new CodeAssetDto 
                { 
                    Id = asset.Id,
                    Name = asset.Name,
                    AssetType = asset.AssetType,
                    Language = asset.Language,
                    Tags = asset.TagsIds.Select(t => new TagDto { Id = t }).ToList(),
                    RootFolder = mergedFolder,
                    PrimaryCodeFile = (CodeFileDto)mergedFolder.Items?.Where(f => f.Name == "Program.cs").FirstOrDefault()!
                };
                
                return result;
            });

        // Act
        var result = await _startProjectsService.CombineStartProjectAsync(startProjectId, CancellationToken.None);

        Console.WriteLine(JsonSerializer.Serialize(result));

        // Assert
        Assert.IsNotNull(result, "Combined asset should not be null.");
        Assert.That(result.Id, Is.EqualTo("combinedAssetId"), "Combined asset Id mismatch.");
        Assert.That(result.Name, Is.EqualTo("Start Project"), "Combined asset Name mismatch.");
        Assert.IsNotNull(result.RootFolder, "Combined asset RootFolder should not be null.");
        Assert.That(result.RootFolder.Name, Is.EqualTo("StartProject"), "Combined RootFolder Name mismatch.");
        Assert.That(result.Tags, Has.Count.EqualTo(4));

        // Since only file content is merged, we expect 6 items:
        // File1.cs, Controllers folder, File2.cs, Services folder, Program.cs and StartProject.csproj.
        Assert.That(result.RootFolder.Items.Count, Is.EqualTo(6), "Expected 6 items in the merged RootFolder.");

        // Verify that File1.cs exists.
        var file1 = result.RootFolder.Items
            .OfType<CodeFileDto>()
            .FirstOrDefault(f => f.Name.Equals("File1.cs", StringComparison.Ordinal));
        Assert.IsNotNull(file1, "File1.cs is missing from the merged RootFolder.");

        // Verify that File2.cs exists.
        var file2 = result.RootFolder.Items
            .OfType<CodeFileDto>()
            .FirstOrDefault(f => f.Name.Equals("File2.cs", StringComparison.Ordinal));
        Assert.IsNotNull(file2, "File2.cs is missing from the merged RootFolder.");

        // Verify the Controllers folder and its file.
        var controllers = result.RootFolder.Items
            .OfType<FolderDto>()
            .FirstOrDefault(f => f.Name.Equals("Controllers", StringComparison.Ordinal));
        Assert.IsNotNull(controllers, "Controllers folder is missing from the merged RootFolder.");
        var homeController = controllers.Items
            .OfType<CodeFileDto>()
            .FirstOrDefault(f => f.Name.Equals("HomeController.cs", StringComparison.Ordinal));
        Assert.IsNotNull(homeController, "HomeController.cs is missing in the Controllers folder.");

        // Verify the Services folder and its files.
        var services = result.RootFolder.Items
            .OfType<FolderDto>()
            .FirstOrDefault(f => f.Name.Equals("Services", StringComparison.Ordinal));
        Assert.IsNotNull(services, "Services folder is missing from the merged RootFolder.");
        var iMyService = services.Items
            .OfType<CodeFileDto>()
            .FirstOrDefault(f => f.Name.Equals("IMyService.cs", StringComparison.Ordinal));
        Assert.IsNotNull(iMyService, "IMyService.cs is missing in the Services folder.");
        var myService = services.Items
            .OfType<CodeFileDto>()
            .FirstOrDefault(f => f.Name.Equals("MyService.cs", StringComparison.Ordinal));
        Assert.IsNotNull(myService, "MyService.cs is missing in the Services folder.");

        // Verify that Program.cs exists.
        var program = result.RootFolder.Items
            .OfType<CodeFileDto>()
            .FirstOrDefault(f => f.Name.Equals("Program.cs", StringComparison.Ordinal));
        Assert.IsNotNull(program, "Program.cs is missing from the merged RootFolder.");

        // Verify the generated csproj file.
        var csproj = result.RootFolder.Items
            .OfType<CodeFileDto>()
            .FirstOrDefault(f => f.Name.Equals("StartProject.csproj", StringComparison.Ordinal));
        Assert.IsNotNull(csproj, "StartProject.csproj is missing from the merged RootFolder.");
        Assert.That(csproj.Language, Is.EqualTo("XML"), "The csproj file language is incorrect.");
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
