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
using AssetsManagerApi.Application.Utils;
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

    public async Task<CodeFileDto> DeleteCodeFileAsync(string startProjectId, string codeFileId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting code file {codeFileId} for start project {startProjectId}", codeFileId, startProjectId);

        var codeFile = await _codeFilesService.DeleteCodeFileAsync(codeFileId, cancellationToken);

        _logger.LogInformation("Deleted code file with ID {codeFileId}", codeFileId);

        return codeFile;
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

    public async Task<FolderDto> DeleteFolderAsync(string startProjectId, string folderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting folder {folderId} for start project {startProjectId}", folderId, startProjectId);

        var folder = await _foldersService.DeleteFolderAsync(folderId, cancellationToken);

        _logger.LogInformation("Deleted folder with ID {folderId}", folderId);

        return folder;
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
        var startProjectName = "Start Project";
        var startProjectNamePascalCase = "StartProject";

        var combinedAssetCreateDto = new CodeAssetCreateDto
        {
            Name = startProjectName,
            AssetType = AssetTypes.Corporate,
            // TODO: update to dynamic. Currently only C# is supported
            Language = Languages.csharp.LanguageToString(),
            RootFolderName = startProjectNamePascalCase
        };

        var combinedAsset = await _codeAssetsService.CreateCodeAssetAsync(combinedAssetCreateDto, cancellationToken);

        _logger.LogInformation("Shell for combined asset is created.");

        var tags = new List<TagDto>();
        var rootFolders = new List<FolderDto>();
        foreach (var assetId in startProject.CodeAssetsIds)
        {
            var asset = await _codeAssetsService.GetCodeAssetAsync(assetId, cancellationToken);
            tags.AddRange(asset.Tags);
            rootFolders.Add(asset.RootFolder);
        }

        var mergedFolder = FolderMerger.MergeFolders(rootFolders);
        mergedFolder.Name = startProjectNamePascalCase;

        // TODO: modify existing folder, do not create a copy
        var (updatedNamespaceFolder, removedNamespaces) = CSharpFileTransformer.UpdateNamespaces(mergedFolder);

        var removedUsingsFolder = CSharpFileTransformer.RemoveInvalidUsings(updatedNamespaceFolder, removedNamespaces);
        var allCodeFiles = GetAllCodeFilesFromFolder(removedUsingsFolder);

        var csprojFile = await CreateCsprojAsync(allCodeFiles, cancellationToken);
        csprojFile.Name = $"{startProjectNamePascalCase}.csproj";
        removedUsingsFolder.Items?.Add(csprojFile);

        var typeToNamespaceMappings = CSharpFileTransformer.BuildClassToNamespaceDictionary(removedUsingsFolder);
        var newUsingFolder = CSharpFileTransformer.AddMissingUsingsToFolder(removedUsingsFolder, typeToNamespaceMappings);

        var programFile = ProgramCsGenerator.GenerateProgramCs(newUsingFolder, startProjectNamePascalCase);
        newUsingFolder.Items?.Add(programFile);

        await _foldersService.SaveFolderHierarchyAsync(newUsingFolder, null, cancellationToken);

        var updateCodeAsset = new CodeAssetUpdateDto
        {
            Id = combinedAsset.Id,
            Name = combinedAsset.Name,
            AssetType = combinedAsset.AssetType,
            Language = combinedAsset.Language,
            TagsIds = tags.Select(t => t.Id).ToList(),
            RootFolderId = newUsingFolder.Id,
            PrimaryCodeFileId = newUsingFolder.Items?.Where(f => f.Name == "Program.cs").FirstOrDefault()?.Id
        };
        var updatedCodeAsset = await _codeAssetsService.UpdateCodeAssetAsync(updateCodeAsset, cancellationToken);

        startProject.CodeAssetId = updatedCodeAsset.Id;
        await _startProjectsRepository.UpdateAsync(startProject, cancellationToken);

        _logger.LogInformation("Successfully combined start project {startProjectId}", startProjectId);

        return updatedCodeAsset;
    }


    private List<CodeFileDto> GetAllCodeFilesFromFolder(FolderDto folder)
    {
        var codeFiles = new List<CodeFileDto>();

        if (folder?.Items == null)
            return codeFiles;

        foreach (var node in folder.Items)
        {
            if (node.Type == FileType.CodeFile)
            {
                codeFiles.Add((CodeFileDto)node);
            }
            else if (node is FolderDto subfolder)
            {
                codeFiles.AddRange(GetAllCodeFilesFromFolder(subfolder));
            }
        }

        return codeFiles;
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

        var compilationResult = codeAsset.Language.StringToLanguage() switch
        {
            Languages.csharp => await _compilationService.CompileDotNetProjectAsync(zipContent, cancellationToken),
            _ => throw new NotImplementedException($"Compilation for {codeAsset.Language} is not implemented."),
        };

        // TODO: Add tips from AI
        var code = CombineCodeFilesText(codeAsset.RootFolder);
        var drySolidReview = await _generativeAiService.ReviewCodeOnDrySolidPrinciplesAsync(code, cancellationToken);
        compilationResult.Output += $"DRY & SOLID Code Review\n{drySolidReview}";

        return compilationResult;
    }

    /// <summary>
    /// Combines the text from all code files in the folder hierarchy.
    /// A couple of new lines are inserted between each file's content.
    /// </summary>
    /// <param name="folder">The root folder to start traversing.</param>
    /// <returns>The concatenated code text.</returns>
    public string CombineCodeFilesText(FolderDto folder)
    {
        if (folder == null)
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();
        TraverseFolderHierarchy(folder, stringBuilder);
        return stringBuilder.ToString();
    }

    private void TraverseFolderHierarchy(FolderDto folder, StringBuilder stringBuilder)
    {
        if (folder.Items == null || folder.Items.Count == 0)
        {
            return;
        }

        foreach (var item in folder.Items)
        {
            switch (item.Type)
            {
                case FileType.CodeFile:
                    if (item is CodeFileDto codeFile)
                    {
                        // Append the code file text followed by a couple of new lines
                        stringBuilder.AppendLine(codeFile.Text);
                        stringBuilder.AppendLine();
                    }
                    break;
                case FileType.Folder:
                    if (item is FolderDto childFolder)
                    {
                        // Recursive call to traverse the child folder
                        TraverseFolderHierarchy(childFolder, stringBuilder);
                    }
                    break;
            }
        }
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

    public async Task<CodeFileDto> CreateCsprojAsync(IEnumerable<CodeFileDto> files, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating csproj file");

        var packages = new HashSet<string>();
        foreach (var file in files)
        {
            var lines = file.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
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

        return new CodeFileDto { Text = sb.ToString(), Language = Languages.xml.LanguageToString(), Type = FileType.CodeFile };
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
                    var folder = (FolderDto)file;
                    var createFolderDto = new FolderCreateDto
                    {
                        Name = file.Name,
                        ParentId = parentId,
                    };
                    var newFolder = await _foldersService.CreateFolderAsync(createFolderDto, cancellationToken);

                    await AddFilesFromFolderAsync(newFolder.Id, folder.Items, cancellationToken);
                    break;

                case FileType.CodeFile:
                    var codeFile = (CodeFileDto)file;
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
