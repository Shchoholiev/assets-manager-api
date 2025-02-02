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

    private readonly IUsersRepository _usersRepository;

    private readonly IMapper _mapper;

    public CodeAssetsService(ICodeAssetsRepository codeAssetsRepository, IFoldersRepository foldersRepository, IUsersRepository usersRepository, IMapper mapper)
    {
        _codeAssetsRepository = codeAssetsRepository;
        _foldersRepository = foldersRepository;
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public async Task<CodeAssetDto> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken)
    {
        var entity = await this._codeAssetsRepository.GetOneAsync(codeAssetId, cancellationToken);
        var folder = await this._foldersRepository.GetOneAsync(entity.RootFolderId, cancellationToken);
        var user = await this._usersRepository.GetOneAsync(entity.CreatedById, cancellationToken);
        
        var dto = _mapper.Map<CodeAssetDto>(entity);

        dto.RootFolder = _mapper.Map<FolderDto>(folder);
        dto.User = _mapper.Map<UserDto>(user);

        return dto;
    }

    public async Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(CodeAssetFilterModel filterModel, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        Expression<Func<CodeAsset, bool>> predicate = codeAsset => codeAsset.AssetType == filterModel.AssetType;

        if (filterModel.AssetType == AssetTypes.Private)
        {
            predicate.And(codeAsset => codeAsset.CreatedById == GlobalUser.Id);
        }

        if (filterModel.AssetType == AssetTypes.Public)
        {
            predicate.And(codeAsset => codeAsset.CompanyId == null);
        }

        if (filterModel.AssetType == AssetTypes.Corporate)
        {
            predicate.And(codeAsset => codeAsset.CompanyId == GlobalUser.CompanyId);
        }

        if (filterModel.TagIds != null)
        {
            predicate.And(codeAsset => codeAsset.Tags.Any(tag => filterModel.TagIds.Contains(tag.Id)));
        }

        if (filterModel.SearchString != null)
        {
            var input = filterModel.SearchString.ToLower();
            predicate.And(codeAsset => codeAsset.Name.ToLower().Contains(input) || codeAsset.Description.ToLower().Contains(input));
        }

        switch(filterModel.Language)
        {
            case "Javascript":
                predicate.And(codeAsset => codeAsset.Language == Languages.javascript);
                break;

            case "Csharp":
                predicate.And(codeAsset => codeAsset.Language == Languages.csharp);
                break;

            case "Python":
                predicate.And(codeAsset => codeAsset.Language == Languages.python);
                break;
        }

        var entities = await _codeAssetsRepository.GetPageAsync(pageNumber, pageSize, predicate, cancellationToken);

        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);

        for (var i = 0; i < entities.Count; i++)
        {
            var folder = await _foldersRepository.GetOneAsync(entities[i].RootFolderId, cancellationToken);
            dtos[i].RootFolder = _mapper.Map<FolderDto>(folder);

            var user = await _usersRepository.GetOneAsync(entities[i].CreatedById, cancellationToken);
            dtos[i].User = _mapper.Map<UserDto>(user);
        }

        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);

        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
    }
}
