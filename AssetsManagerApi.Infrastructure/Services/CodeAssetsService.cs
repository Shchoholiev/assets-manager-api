using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Global;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using AutoMapper;
using LinqKit;
using System.Linq.Expressions;

namespace AssetsManagerApi.Infrastructure.Services;
public class CodeAssetsService : ICodeAssetsService
{
    private readonly ICodeAssetsRepository _codeAssetsRepository;

    private readonly IFoldersRepository _foldersRepository;

    private readonly ICodeFilesRepository _codeFilesRepository;

    private readonly IUsersRepository _usersRepository;

    private readonly IMapper _mapper;

    public CodeAssetsService(ICodeAssetsRepository codeAssetsRepository, IFoldersRepository foldersRepository, IUsersRepository usersRepository, ICodeFilesRepository codeFilesRepository, IMapper mapper)
    {
        _codeAssetsRepository = codeAssetsRepository;
        _foldersRepository = foldersRepository;
        _usersRepository = usersRepository;
        _codeFilesRepository = codeFilesRepository;
        _mapper = mapper;
    }

    public async Task<CodeAssetDto> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken)
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

    public async Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(CodeAssetFilterModel filterModel, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        Expression<Func<CodeAsset, bool>> predicate = codeAsset => codeAsset.AssetType == filterModel.AssetType;

        if (filterModel.AssetType == AssetTypes.Private)
        {
            predicate = predicate.And(codeAsset => codeAsset.CreatedById == GlobalUser.Id);
        }

        if (filterModel.AssetType == AssetTypes.Public)
        {
            predicate = predicate.And(codeAsset => codeAsset.CompanyId == null);
        }

        if (filterModel.AssetType == AssetTypes.Corporate)
        {
            predicate = predicate.And(codeAsset => codeAsset.CompanyId == GlobalUser.CompanyId);
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

        switch(filterModel.Language)
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
        }

        var entities = await _codeAssetsRepository.GetPageAsync(pageNumber, pageSize, predicate, cancellationToken);

        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);

        for (var i = 0; i < entities.Count; i++)
        {
            var primaryCodeFile = await this._codeFilesRepository.GetOneAsync(entities[i].PrimaryCodeFileId, cancellationToken);
            var folder = await this._foldersRepository.GetOneAsync(entities[i].RootFolderId, cancellationToken);
            var user = await this._usersRepository.GetOneAsync(entities[i].CreatedById, cancellationToken);

            dtos[i].UserName = user.Name;
            dtos[i].PrimaryCodeFile = _mapper.Map<CodeFileDto>(primaryCodeFile);
            dtos[i].RootFolder = await ComposeRootFolder(folder, cancellationToken);
        }

        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);

        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
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
