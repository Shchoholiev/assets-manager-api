using System.Text;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Global;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AssetsManagerApi.Infrastructure.Services;

public class StartProjectsService(
    ICodeAssetsService codeAssetsService,
    IGenerativeAiService generativeAiService,
    IStartProjectsRepository startProjectsRepository,
    ICodeFilesService codeFilesService,
    IFoldersService foldersService,
    ILogger<StartProjectsService> logger,
    INugetService nugetService
) : IStartProjectsService
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    private readonly IGenerativeAiService _generativeAiService = generativeAiService;

    private readonly IStartProjectsRepository _startProjectsRepository = startProjectsRepository;

    private readonly ICodeFilesService _codeFilesService = codeFilesService;

    private readonly IFoldersService _foldersService = foldersService;

    private readonly ILogger<StartProjectsService> _logger = logger;

    private readonly INugetService _nugetService = nugetService;

    public async Task<StartProjectDto> CreateStartProjectAsync(StartProjectCreateDto createDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating start project");

        if (string.IsNullOrEmpty(GlobalUser.CompanyId))
        {
            _logger.LogError("CompanyId is null");
            throw new UnauthorizedAccessException("User does not belong to any company");
        }

        var filterModel = new CodeAssetFilterModel
        {
            AssetType = AssetTypes.Corporate,
            Language = createDto.Language.LanguageToString()
        };

        _logger.LogInformation("Chosen Language: {language}", filterModel.Language);

        var assets = await _codeAssetsService.GetCodeAssetsPageAsync(filterModel, 1, 100, cancellationToken);

        _logger.LogInformation("Retrieved {count} code assets", assets.Items.Count());

        var selectedAssets = await _generativeAiService.SelectRelevantCodeAssets(
            createDto.Prompt,
            assets.Items,
            cancellationToken);

        var startProject = new StartProject
        {
            CodeAssetsIds = selectedAssets.Select(x => x.Id).ToList(),
            CompanyId = GlobalUser.CompanyId,
            CreatedDateUtc = DateTime.UtcNow,
            CreatedById = GlobalUser.Id
        };
        var newStartProject = await _startProjectsRepository.AddAsync(startProject, cancellationToken);

        return new StartProjectDto
        {
            Id = newStartProject.Id.ToString(),
            CodeAssets = selectedAssets
        };
    }

    public async Task<CodeFileDto> CreateCodeFileAsync(string startProjectId, CodeFileCreateDto codeFileDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating code file for start project {startProjectId}", startProjectId);

        var createdCodeFile = await _codeFilesService.CreateCodeFileAsync(codeFileDto, cancellationToken);

        _logger.LogInformation("Created code file with ID {codeFileId}", createdCodeFile.Id);

        return createdCodeFile;
    }

    public async Task<CodeFileDto> UpdateCodeFileAsync(string startProjectId, string codeFileId, CodeFileUpdateDto codeFileDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating code file {codeFileId} for start project {startProjectId}", codeFileId, startProjectId);

        codeFileDto.Id = string.IsNullOrEmpty(codeFileDto.Id) ? codeFileId : codeFileDto.Id;
        var updatedCodeFile = await _codeFilesService.UpdateCodeFileAsync(codeFileDto, cancellationToken);

        _logger.LogInformation("Updated code file with ID {codeFileId}", updatedCodeFile.Id);

        return updatedCodeFile;
    }

    public async Task DeleteCodeFileAsync(string startProjectId, string codeFileId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting code file {codeFileId} for start project {startProjectId}", codeFileId, startProjectId);

        await _codeFilesService.DeleteCodeFileAsync(codeFileId, cancellationToken);

        _logger.LogInformation("Deleted code file with ID {codeFileId}", codeFileId);
    }

    public async Task<FolderDto> CreateFolderAsync(string startProjectId, FolderCreateDto folderDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating folder for start project {startProjectId}", startProjectId);

        var createdFolder = await _foldersService.CreateFolderAsync(folderDto, cancellationToken);

        _logger.LogInformation("Created folder with ID {folderId}", createdFolder.Id);

        return createdFolder;
    }

    public async Task<FolderDto> UpdateFolderAsync(string startProjectId, string folderId, FolderUpdateDto folderDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating folder {folderId} for start project {startProjectId}", folderId, startProjectId);

        var updatedFolder = await _foldersService.UpdateFolderAsync(folderDto, cancellationToken);

        _logger.LogInformation("Updated folder with ID {folderId}", updatedFolder.Id);

        return updatedFolder;
    }

    public async Task DeleteFolderAsync(string startProjectId, string folderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting folder {folderId} for start project {startProjectId}", folderId, startProjectId);

        await _foldersService.DeleteFolderAsync(folderId, cancellationToken);

        _logger.LogInformation("Deleted folder with ID {folderId}", folderId);
    }

    public async Task<CodeAssetDto> CombineStartProjectAsync(string startProjectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Combining start project {startProjectId}", startProjectId);

        var startProject = await _startProjectsRepository.GetOneAsync(startProjectId, cancellationToken);
        if (startProject == null)
        {
            _logger.LogError("Start project {startProjectId} not found", startProjectId);
            throw new InvalidOperationException("Start project not found.");
        }

        var combinedAssetCreateDto = new CodeAssetCreateDto 
        {
            Name = "Start Project", // To be updated
            AssetType = AssetTypes.Corporate,
            // TODO: update to dynamic. Currently only C# is supported
            Language = Languages.csharp.LanguageToString(), 
        };

        var combinedAsset = await _codeAssetsService.CreateCodeAssetAsync(combinedAssetCreateDto, cancellationToken);

        _logger.LogInformation("Shell for combined asset is created.");

        // var tags = new List<Tag>();
        // var folders = new List<Folder>();
        // var codeFiles = new List<CodeFile>();
        // foreach (var assetId in startProject.CodeAssetsIds)
        // {
        //     var asset = await _codeAssetsService.GetCodeAssetAsync(assetId, cancellationToken);
        //     foreach (var folder in asset.RootFolder.Items ?? [])
        //     {
        //         if (!allFolders.TryGetValue(folder.Name, out var existingFolder))
        //         {
        //             allFolders[folder.Name] = folder;
        //         }
        //         else
        //         {
        //             // Merge files into existing folder
        //             existingFolder.CodeFiles.AddRange(folder.CodeFiles);
        //         }
        //     }

        //     allFiles.AddRange(asset.Folders.SelectMany(f => f.CodeFiles));
        // }

        // // Step 3: Create .csproj file from flat list of all code files
        // var csprojFile = await CreateCsprojAsync(allFiles, cancellationToken);
        // csprojFile.FileName = "Project.csproj";

        // // Step 4: Add .csproj file to root folder
        // var rootFolder = allFolders.Values.FirstOrDefault(f => f.ParentFolderId == null);
        // if (rootFolder == null)
        // {
        //     // If no explicit root folder, create one
        //     rootFolder = new FolderDto
        //     {
        //         Name = "root",
        //         CodeFiles = new List<CodeFileDto>(),
        //         SubFolders = new List<FolderDto>()
        //     };
        //     allFolders["root"] = rootFolder;
        // }

        // rootFolder.CodeFiles.Add(csprojFile);

        // Step 5: Return a combined CodeAssetDto
        // var combinedAsset = new CodeAssetDto
        // {
        //     Name = $"CombinedProject_{startProjectId}",
        //     Folders = allFolders.Values.ToList()
        // };
        startProject.CodeAssetId = combinedAsset.Id;
        await _startProjectsRepository.UpdateAsync(startProject, cancellationToken);

        _logger.LogInformation("Successfully combined start project {startProjectId}", startProjectId);

        return combinedAsset;
    }


    public Task<CompilationResult> CompileStartProjectAsync(string startProjectId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> DownloadStartProjectAsync(string startProjectId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<CodeFileDto> CreateCsprojAsync(IEnumerable<CodeFileDto> files, CancellationToken cancellationToken)
    {
        var packages = new HashSet<string>();
        foreach (var file in files)
        {
            var lines = file.Text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("using ") && trimmed.EndsWith(";"))
                {
                    var ns = trimmed.Substring(6, trimmed.Length - 7).Trim();
                    if (!ns.StartsWith("System"))
                    {
                        packages.Add(ns);
                    }
                }
            }
        }

        var sb = new StringBuilder();
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net8.0</TargetFramework>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("  </PropertyGroup>");

        if (packages.Count != 0)
        {
            sb.AppendLine("  <ItemGroup>");
            foreach (var package in packages)
            {
                var version = await _nugetService.GetPackageLatestVersionAsync(package, cancellationToken);
                sb.AppendLine($"    <PackageReference Include=\"{package}\" Version=\"{version}\" />");
            }
            sb.AppendLine("  </ItemGroup>");
        }
        sb.AppendLine("</Project>");

        return new CodeFileDto { Text = sb.ToString() };
    }

    public Task<CodeAssetDto> GetCombinedAssetAsync(string startProjectId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
