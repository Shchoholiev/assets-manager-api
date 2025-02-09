using System.Text.Json;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Global;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AssetsManagerApi.Infrastructure.Services;

public class StartProjectsService(
    ICodeAssetsService codeAssetsService,
    IGenerativeAiService generativeAiService,
    IStartProjectsRepository startProjectsRepository,
    ILogger<StartProjectsService> logger
) : IStartProjectsService
{
    private readonly ICodeAssetsService _codeAssetsService = codeAssetsService;

    private readonly IGenerativeAiService _generativeAiService = generativeAiService;

    private readonly IStartProjectsRepository _startProjectsRepository = startProjectsRepository;

    private readonly ILogger<StartProjectsService> _logger = logger;

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
            CodeAssetsIds = selectedAssets.Select(x => x.Id).ToList()
        };
        var newStartProject = await _startProjectsRepository.AddAsync(startProject, cancellationToken);

        return new StartProjectDto
        {
            Id = newStartProject.Id.ToString(),
            CodeAssets = selectedAssets
        };
    }

    public Task<CodeFileDto> CreateCodeFileAsync(string startProjectId, CodeFileDto codeFileDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<FolderDto> CreateFolderAsync(string startProjectId, FolderDto folderDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CodeFileDto> UpdateCodeFileAsync(string startProjectId, string codeFileId, CodeFileDto codeFileDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<FolderDto> UpdateFolderAsync(string startProjectId, string folderId, FolderDto folderDto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCodeFileAsync(string startProjectId, string codeFileId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFolderAsync(string startProjectId, string folderId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CodeAssetDto> CombineStartProjectAsync(string startProjectId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CompilationResult> CompileStartProjectAsync(string startProjectId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> DownloadStartProjectAsync(string startProjectId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
