using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Global;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using AutoMapper;
using LinqKit;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Linq.Expressions;

namespace AssetsManagerApi.Infrastructure.Services;
public class CodeAssetsService : ICodeAssetsService
{
    private readonly ICodeAssetsRepository _codeAssetsRepository;

    private readonly IFoldersRepository _foldersRepository;

    private readonly ICodeFilesRepository _codeFilesRepository;

    private readonly IUsersRepository _usersRepository;

    private readonly IFoldersService _foldersService;

    private readonly ITagsRepository _tagsRepository;

    private readonly ILogger<CodeAssetsService> _logger;

    private readonly IMapper _mapper;

    public CodeAssetsService(
        ICodeAssetsRepository codeAssetsRepository, IFoldersRepository foldersRepository, 
        IUsersRepository usersRepository, ICodeFilesRepository codeFilesRepository, IFoldersService foldersService, 
        ITagsRepository tagsRepository, IMapper mapper,
        ILogger<CodeAssetsService> logger)
    {
        _tagsRepository = tagsRepository;
        _foldersService = foldersService;
        _codeAssetsRepository = codeAssetsRepository;
        _foldersRepository = foldersRepository;
        _usersRepository = usersRepository;
        _codeFilesRepository = codeFilesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CodeAssetDto> CreateCodeAssetAsync(CodeAssetCreateDto createDto, CancellationToken cancellationToken)
    {
        if (createDto.AssetType == AssetTypes.Corporate && !GlobalUser.Roles.Contains("Enterprise"))
        {
            throw new AccessViolationException("You are not enterprise user");
        }

        if (!(createDto.AssetType == AssetTypes.Corporate) && GlobalUser.Roles.Contains("Enterprise"))
        {
            throw new AccessViolationException("Enterprise users can create only corporate assets");
        }

        var folder = new Folder()
        {
            Name = createDto.RootFolderName,
            Type = FileType.Folder,
            CreatedById = GlobalUser.Id,
            CreatedDateUtc = DateTime.UtcNow,
        };

        folder = await _foldersRepository.AddAsync(folder, cancellationToken);

        var primaryCodeFIle = new CodeFile
        {
            Name = createDto.PrimaryCodeFileName,
            Type = FileType.CodeFile,
            Text = "",
            Language = LanguagesExtensions.StringToLanguage(createDto.Language),
            ParentId = folder.Id,
            CreatedById = GlobalUser.Id,
            CreatedDateUtc = DateTime.UtcNow,
        };

        primaryCodeFIle = await _codeFilesRepository.AddAsync(primaryCodeFIle, cancellationToken);

        var entity = new CodeAsset()
        {
            Description = "",
            Name = createDto.Name,
            AssetType = createDto.AssetType,
            CreatedById = GlobalUser.Id,
            CreatedDateUtc = DateTime.UtcNow,
            Language = LanguagesExtensions.StringToLanguage(createDto.Language),
            PrimaryCodeFileId = primaryCodeFIle.Id,
            RootFolderId = folder.Id,
        };

        if (createDto.AssetType == AssetTypes.Corporate)
        {
            entity.CompanyId = GlobalUser.CompanyId;
        }
        else
        {
            entity.CompanyId = null;
        }

        var result = await _codeAssetsRepository.AddAsync(entity, cancellationToken);

        var resultDto = _mapper.Map<CodeAssetDto>(result);

        resultDto.UserName = GlobalUser.Name;
        resultDto.PrimaryCodeFile = _mapper.Map<CodeFileDto>(primaryCodeFIle);
        resultDto.RootFolder = await ComposeRootFolder(folder, cancellationToken);

        return resultDto;
    }

    public async Task<CodeAssetDto> DeleteCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken)
    {
        var asset = await _codeAssetsRepository.GetOneAsync(codeAssetId, cancellationToken);
        if (asset == null)
        {
            throw new EntityNotFoundException("Code asset not found");
        }

        var folder = await _foldersService.DeleteFolderAsync(asset.RootFolderId, cancellationToken);
        if (folder == null)
        {
            throw new EntityNotFoundException("Root folder not found");
        }

        await _codeAssetsRepository.DeleteAsync(asset, cancellationToken);

        return _mapper.Map<CodeAssetDto>(asset);
    }

    public async Task<CodeAssetDto> GetCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken)
    {
        var entity = await this._codeAssetsRepository.GetOneAsync(codeAssetId, cancellationToken);
        var primaryCodeFile = await this._codeFilesRepository.GetOneAsync(entity.PrimaryCodeFileId, cancellationToken);
        var folder = await this._foldersRepository.GetOneAsync(entity.RootFolderId, cancellationToken);
        var user = await this._usersRepository.GetOneAsync(entity.CreatedById, cancellationToken);

        var dto = _mapper.Map<CodeAssetDto>(entity);
        dto.UserName = user.Name;
        dto.PrimaryCodeFile = _mapper.Map<CodeFileDto>(primaryCodeFile);
        dto.RootFolder = await ComposeRootFolder(folder, cancellationToken);

        return dto;
    }

    public async Task<PagedList<CodeAssetDto>> GetCodeAssetsPageAsync(CodeAssetFilterModel filterModel, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        Expression<Func<CodeAsset, bool>> predicate = CodeAsset => true;

        if (filterModel.IsPersonal)
        {
            predicate = predicate.And(codeAsset => codeAsset.CreatedById == GlobalUser.Id)
                                 .And(codeAsset => codeAsset.AssetType == AssetTypes.Public || codeAsset.AssetType == AssetTypes.Corporate);
        }
        else
        {
            if (filterModel.AssetType == AssetTypes.Public)
            {
                predicate = predicate.And(codeAsset => codeAsset.CompanyId == null || codeAsset.CompanyId == "")
                                     .And(codeAsset => codeAsset.AssetType == AssetTypes.Public);
            }

            if (filterModel.AssetType == AssetTypes.Corporate)
            {
                predicate = predicate.And(codeAsset => codeAsset.CompanyId == GlobalUser.CompanyId)
                                     .And(codeAsset => codeAsset.AssetType == AssetTypes.Corporate);
            }
        }

        if (filterModel.TagIds != null)
        {
            predicate = predicate.And(codeAsset => codeAsset.Tags.Any(tag => filterModel.TagIds.Contains(tag.Id)));
        }

        if (filterModel.SearchString != null)
        {
            var input = filterModel.SearchString.ToLower();
            predicate = predicate.And(codeAsset => codeAsset.Name.ToLower().Contains(input) || codeAsset.Description.ToLower().Contains(input));
        }

        switch (filterModel.Language)
        {
            case "Javascript":
                predicate = predicate.And(codeAsset => codeAsset.Language == Languages.javascript);
                break;

            case "Csharp":
                predicate = predicate.And(codeAsset => codeAsset.Language == Languages.csharp);
                break;

            case "Python":
                predicate = predicate.And(codeAsset => codeAsset.Language == Languages.python);
                break;

            case null:
                break;

            default:
                throw new ArgumentException($"Invalid language: {filterModel.Language}");
        }

        var entities = await _codeAssetsRepository.GetPageAsync(pageNumber, pageSize, predicate, cancellationToken);

        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);

        for (var i = 0; i < entities.Count; i++)
        {
            var primaryCodeFile = await this._codeFilesRepository.GetOneAsync(entities[i].PrimaryCodeFileId, cancellationToken);
            var user = await this._usersRepository.GetOneAsync(entities[i].CreatedById, cancellationToken);

            dtos[i].UserName = user.Name;
            dtos[i].PrimaryCodeFile = _mapper.Map<CodeFileDto>(primaryCodeFile);
        }

        var totalCount = await this._codeAssetsRepository.GetCountAsync(predicate, cancellationToken);

        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<CodeAssetDto> UpdateCodeAssetAsync(CodeAssetUpdateDto dto, CancellationToken cancellationToken)
    {
        var codeAsset = await _codeAssetsRepository.GetOneAsync(dto.Id, cancellationToken);

        if (codeAsset == null)
        {
            throw new EntityNotFoundException("Code asset not found");
        }

        codeAsset.Description = dto.Description;    
        codeAsset.Name = dto.Name;
        codeAsset.AssetType = dto.AssetType;
        codeAsset.RootFolderId = dto.RootFolderId;
        codeAsset.PrimaryCodeFileId = dto.PrimaryCodeFileId;
        codeAsset.Language = LanguagesExtensions.StringToLanguage(dto.Language);
        codeAsset.Tags = new List<Tag>();
        codeAsset.LastModifiedById = GlobalUser.Id;
        codeAsset.LastModifiedDateUtc = DateTime.UtcNow;

        foreach (var tagId in dto.TagsIds)
        {
            var tag = await _tagsRepository.GetOneAsync(tagId, cancellationToken)
                      ?? throw new EntityNotFoundException($"Tag with ID {tagId} not found");
            codeAsset.Tags.Add(tag);
        }

        var entity = await _codeAssetsRepository.UpdateAsync(codeAsset, cancellationToken);

        var result = _mapper.Map<CodeAssetDto>(entity);

        result.UserName = GlobalUser.Name;

        var folder = await _foldersRepository.GetOneAsync(entity.RootFolderId, cancellationToken);

        if (folder == null)
        {
            throw new EntityNotFoundException("Root folder not found");
        }

        var primaryCodeFIle = await _codeFilesRepository.GetOneAsync(entity.PrimaryCodeFileId, cancellationToken);

        if (primaryCodeFIle == null)
        {
            throw new EntityNotFoundException("Primary code file not found");
        }

        result.PrimaryCodeFile = _mapper.Map<CodeFileDto>(primaryCodeFIle);
        result.RootFolder = await ComposeRootFolder(folder, cancellationToken);

        return result;
    }


    public async Task<(byte[] zipContent, string fileName)> GetCodeAssetAsZipAsync(string assetId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting ZIP creation for code asset with Id: {AssetId}", assetId);

        var codeAsset = await GetCodeAssetAsync(assetId, cancellationToken);

        if (codeAsset == null)
        {
            _logger.LogError("Code asset with Id {AssetId} not found.", assetId);
            throw new EntityNotFoundException("Code asset not found.");
        }

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            _logger.LogInformation("Adding files to ZIP for code asset: {AssetName}", codeAsset.Name);
            AddFilesToArchiveRecursive(archive, codeAsset.RootFolder.Items ?? [], basePath: string.Empty);
        }

        _logger.LogInformation("Finished ZIP creation for code asset: {AssetName}", codeAsset.Name);

        return (memoryStream.ToArray(), $"{codeAsset.Name}.zip");
    }


    private void AddFilesToArchiveRecursive(ZipArchive archive, List<FileSystemNodeDto> files, string basePath)
    {
        foreach (var file in files)
        {
            switch (file.Type)
            {
                case FileType.Folder:
                    var folder = (FolderDto)file;
                    string folderPath = $"{basePath}{folder.Name}/";
                    _logger.LogInformation("Adding folder to ZIP: {FolderPath}", folderPath);
                    archive.CreateEntry(folderPath); 
                    AddFilesToArchiveRecursive(archive, folder.Items ?? [], folderPath);
                    break;

                case FileType.CodeFile:
                    var codeFile = (CodeFileDto)file;
                    string filePath = $"{basePath}{codeFile.Name}";
                    _logger.LogInformation("Adding file to ZIP: {FilePath}", filePath);
                    var zipEntry = archive.CreateEntry(filePath);
                    using (var entryStream = zipEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        writer.Write(codeFile.Text);
                    }
                    break;

                default:
                    _logger.LogWarning("Unsupported file type encountered. Skipping: {FileName}", file.Name);
                    break;
            }
        }
    }

    private async Task<FolderDto> ComposeRootFolder(Folder rootFolder, CancellationToken cancellationToken)
    {
        var result = _mapper.Map<FolderDto>(rootFolder);
        result.Items ??= new List<FileSystemNodeDto>();

        var childCodeFiles = await _codeFilesRepository.GetAllAsync(codeFile => codeFile.ParentId == rootFolder.Id, cancellationToken);
        if (childCodeFiles?.Any() == true)
        {
            var childCodeFilesDtos = _mapper.Map<List<CodeFileDto>>(childCodeFiles);
            result.Items.AddRange(childCodeFilesDtos);
        }

        var childFolders = await _foldersRepository.GetAllAsync(folder => folder.ParentId == rootFolder.Id, cancellationToken);
        if (childFolders?.Any() == true)
        {
            foreach (var folder in childFolders)
            {
                var proceededFolder = await ComposeRootFolder(folder, cancellationToken);
                result.Items.Add(proceededFolder);
            }
        }

        return result;
    }
}
