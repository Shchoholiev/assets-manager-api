using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities.Identity;
using AutoMapper;

namespace AssetsManagerApi.Infrastructure.Services;
public class CodeAssetsService : ICodeAssetsService
{
    private readonly ICodeAssetsRepository _codeAssetsRepository;

    private readonly IMapper _mapper;

    public CodeAssetsService(ICodeAssetsRepository codeAssetsRepository, IMapper mapper)
    {
        _codeAssetsRepository = codeAssetsRepository;
        _mapper = mapper;
    }

    public async Task<PagedList<CodeAssetResult>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetResult>>(entities);
        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);
        return new PagedList<CodeAssetResult>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<PagedList<CodeAssetResult>> GetUsersCodeAssetsPage(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, c => c.CreatedById == userId, cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetResult>>(entities);
        var totalCount = await this._codeAssetsRepository.GetCountAsync(c => c.CreatedById == userId, cancellationToken);
        return new PagedList<CodeAssetResult>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<PagedList<CodeAssetResult>> GetCodeAssetsByTagsPage(List<string> tagIds, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, c => c.Tags.Any(tag => tagIds.Contains(tag.Id)), cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetResult>>(entities);
        var totalCount = await this._codeAssetsRepository.GetCountAsync(c => c.Tags.Any(tag => tagIds.Contains(tag.Id)), cancellationToken);
        return new PagedList<CodeAssetResult>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<CodeAssetResult> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken)
    {
        var entity = await this._codeAssetsRepository.GetOneAsync(codeAssetId, cancellationToken);
        return _mapper.Map<CodeAssetResult>(entity);
    }

    public async Task<PagedList<CodeAssetResult>> SearchCodeAssetsPage(string input, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, c => c.Name.Contains(input) || c.Description.Contains(input), cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetResult>>(entities);
        var totalCount = await this._codeAssetsRepository.GetCountAsync(c => c.Name.Contains(input) || c.Description.Contains(input), cancellationToken);
        return new PagedList<CodeAssetResult>(dtos, pageNumber, pageSize, totalCount);
    }
}
