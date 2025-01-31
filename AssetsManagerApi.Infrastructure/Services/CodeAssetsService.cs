using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AutoMapper;
using System.Linq.Expressions;

namespace AssetsManagerApi.Infrastructure.Services;
public class CodeAssetsService : ICodeAssetsService
{
    private readonly ICodeAssetsRepository _codeAssetsRepository;

    private readonly IFoldersRepository _foldersRepository;

    private readonly IMapper _mapper;

    public CodeAssetsService(ICodeAssetsRepository codeAssetsRepository, IFoldersRepository foldersRepository, IMapper mapper)
    {
        _codeAssetsRepository = codeAssetsRepository;
        _foldersRepository = foldersRepository;
        _mapper = mapper;
    }

    public async Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);
        for (var i = 0; i < entities.Count; i++)
        {
            var folder = await _foldersRepository.GetOneAsync(entities[i].RootFolderId, cancellationToken);
            dtos[i].RootFolder = _mapper.Map<FolderDto>(folder);
        }
        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);
        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<CodeAssetDto> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken)
    {
        var entity = await this._codeAssetsRepository.GetOneAsync(codeAssetId, cancellationToken);
        return _mapper.Map<CodeAssetDto>(entity);
    }

    public async Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(Expression<Func<CodeAssetDto, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);
        for (var i = 0; i < entities.Count; i++)
        {
            var folder = await _foldersRepository.GetOneAsync(entities[i].RootFolderId, cancellationToken);
            dtos[i].RootFolder = _mapper.Map<FolderDto>(folder);
        }
        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);
        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
    }
}
