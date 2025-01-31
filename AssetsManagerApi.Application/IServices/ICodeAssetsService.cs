using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using System.Linq.Expressions;

namespace AssetsManagerApi.Application.IServices;

public interface ICodeAssetsService
{
    Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(Expression<Func<CodeAssetDto, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<CodeAssetDto> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken);
}
