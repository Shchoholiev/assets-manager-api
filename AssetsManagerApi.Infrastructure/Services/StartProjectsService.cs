using System.Text;
using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Compilation;
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
    INugetService nugetService,
    ICompilationService compilationService
) : IStartProjectsService
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    private readonly IGenerativeAiService _generativeAiService = generativeAiService;

    private readonly IStartProjectsRepository _startProjectsRepository = startProjectsRepository;

    private readonly ICodeFilesService _codeFilesService = codeFilesService;

    private readonly IFoldersService _foldersService = foldersService;

    private readonly ILogger<StartProjectsService> _logger = logger;

    private readonly INugetService _nugetService = nugetService;

    private readonly ICompilationService _compilationService = compilationService;

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
            throw new EntityNotFoundException("Start project not found.");
        }

        // TODO: update to AI generated name
        var startProjectName = "Test API";
        var startProjectNameCamelCase = "TestApi";

        var combinedAssetCreateDto = new CodeAssetCreateDto 
        {
            Name = startProjectName,
            AssetType = AssetTypes.Corporate,
            // TODO: update to dynamic. Currently only C# is supported
            Language = Languages.csharp.LanguageToString(), 
            RootFolderName = startProjectNameCamelCase
        };

        var combinedAsset = await _codeAssetsService.CreateCodeAssetAsync(combinedAssetCreateDto, cancellationToken);

        _logger.LogInformation("Shell for combined asset is created.");

        var tags = new List<TagDto>();
        var allCodeFiles = new List<CodeFileDto>();
        foreach (var assetId in startProject.CodeAssetsIds)
        {
            var asset = await _codeAssetsService.GetCodeAssetAsync(assetId, cancellationToken);
            tags.AddRange(asset.Tags);

            // TODO: it doesn't drill down to subfolders
            allCodeFiles.AddRange(
                asset.RootFolder.Items?
                    .Where(f => f.Type == FileType.CodeFile)
                    .Select(f => (CodeFileDto)f)
                    .ToList() 
                    ?? []
            );

            // await AddFilesFromFolderAsync(combinedAsset.RootFolder.Id, asset.RootFolder.Items ?? [], cancellationToken);
        }

        // if (combinedAsset.Language.StringToLanguage() == Languages.csharp)
        // {
        //     var csprojFile = await CreateCsprojAsync(allCodeFiles, cancellationToken);
        //     // TODO: update to dynamicly generated
        //     csprojFile.Name = "StartProject.csproj";
        //     var createdCsprojFile = await _codeFilesService.CreateCodeFileAsync(csprojFile, cancellationToken);
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


    public async Task<CompilationResponse> CompileStartProjectAsync(string startProjectId, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Compiling start project with Id: {startProjectId}", startProjectId);

        var startProject = await _startProjectsRepository.GetOneAsync(startProjectId, cancellationToken);
        if (startProject == null)
        {
            _logger.LogError("Start project {startProjectId} not found", startProjectId);
            throw new EntityNotFoundException("Start project not found.");
        }
        if (startProject.CodeAssetId == null)
        {
            _logger.LogError("Start project {startProjectId} has no Combined Asset", startProjectId);
            throw new EntityNotFoundException("Start project has no Combined Asset.");
        }

        var codeAsset = await _codeAssetsService.GetCodeAssetAsync(startProject.CodeAssetId, cancellationToken);
        var (zipContent, _) = await _codeAssetsService.GetCodeAssetAsZipAsync(startProject.CodeAssetId, cancellationToken);

        return codeAsset.Language.StringToLanguage() switch
        {
            Languages.csharp => await _compilationService.CompileDotNetProjectAsync(zipContent, cancellationToken),
            _ => throw new NotImplementedException($"Compilation for {codeAsset.Language} is not implemented."),
        };
    }

    public async Task<(byte[] zipContent, string fileName)> DownloadStartProjectZipAsync(string startProjectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading start project with Id: {startProjectId}", startProjectId);

        var startProject = await _startProjectsRepository.GetOneAsync(startProjectId, cancellationToken);
        if (startProject == null)
        {
            _logger.LogError("Start project {startProjectId} not found", startProjectId);
            throw new EntityNotFoundException("Start project not found.");
        }
        if (startProject.CodeAssetId == null)
        {
            _logger.LogError("Start project {startProjectId} has no Combined Asset", startProjectId);
            throw new EntityNotFoundException("Start project has no Combined Asset.");
        }

        var (zipContent, zipName) = await _codeAssetsService.GetCodeAssetAsZipAsync(startProject.CodeAssetId, cancellationToken);

        _logger.LogInformation("Downloaded start project with Id: {startProjectId}", startProjectId);

        return (zipContent, zipName);
    }

    public async Task<CodeFileCreateDto> CreateCsprojAsync(IEnumerable<CodeFileDto> files, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating csproj file");

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

        _logger.LogInformation("Created csproj file");

        return new CodeFileCreateDto { Text = sb.ToString(), Language = Languages.xml.LanguageToString() };
    }

    public async Task<CodeAssetDto> GetCombinedAssetAsync(string startProjectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting combined code asset for start project with Id: {startProjectId}", startProjectId);

        var startProject = await _startProjectsRepository.GetOneAsync(startProjectId, cancellationToken);
        if (startProject == null)
        {
            _logger.LogError("Start project {startProjectId} not found", startProjectId);
            throw new EntityNotFoundException("Start project not found.");
        }
        if (startProject.CodeAssetId == null)
        {
            _logger.LogError("Start project {startProjectId} has no Combined Asset", startProjectId);
            throw new EntityNotFoundException("Start project has no Combined Asset.");
        }

        var codeAsset = await _codeAssetsService.GetCodeAssetAsync(startProject.CodeAssetId, cancellationToken);

        _logger.LogInformation("Returning combined code asset for start project with Id: {startProjectId}", startProjectId);

        return codeAsset;
    }
    
    private async Task AddFilesFromFolderAsync(string parentId, List<FileSystemNodeDto> files, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding files to folder with Id: {parentId}");

        foreach (var file in files)
        {
            switch (file.Type)
            {
                case FileType.Folder:
                    var folder = (FolderDto) file;
                    var createFolderDto = new FolderCreateDto
                    {
                        Name = file.Name,
                        ParentId = parentId,
                    };
                    var newFolder = await _foldersService.CreateFolderAsync(createFolderDto, cancellationToken);

                    await AddFilesFromFolderAsync(newFolder.Id, folder.Items, cancellationToken);
                    break;

                case FileType.CodeFile:
                    var codeFile = (CodeFileDto) file;
                    var createCodeFileDto = new CodeFileCreateDto
                    {
                        Name = codeFile.Name,
                        Language = codeFile.Language,
                        ParentId = parentId,
                        Text = codeFile.Text
                    };
                    await _codeFilesService.CreateCodeFileAsync(createCodeFileDto, cancellationToken);

                    break;

                default:
                    _logger.LogInformation("File type is not supported.");
                    break;
            }
        }

        _logger.LogInformation($"Finished adding files to folder with Id: {parentId}");
    }
}
