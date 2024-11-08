using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;
using System.Linq;
using System.Linq.Expressions;

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

    public async Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);
        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);
        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<PagedList<CodeAssetDto>> GetUsersCodeAssetsPage(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, c => c.CreatedById == userId, cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);
        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);
        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<PagedList<CodeAssetDto>> GetCodeAssetsByTagsPage(List<string> tagIds, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await this._codeAssetsRepository.GetPageAsync(pageNumber, pageSize, c => c.Tags.Any(tag => tagIds.Contains(tag.Id)), cancellationToken);
        var dtos = _mapper.Map<List<CodeAssetDto>>(entities);
        var totalCount = await this._codeAssetsRepository.GetCountAsync(cancellationToken);
        return new PagedList<CodeAssetDto>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<CodeAssetDto> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken)
    {
        var entity = await this._codeAssetsRepository.GetOneAsync(codeAssetId, cancellationToken);
        return _mapper.Map<CodeAssetDto>(entity);
    }
}
