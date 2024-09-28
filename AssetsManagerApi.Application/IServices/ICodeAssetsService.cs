using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using System.Linq.Expressions;

namespace AssetsManagerApi.Application.IServices;

public interface ICodeAssetsService
{
    Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(int pageNumber, int pageSize, Expression<Func<CodeAsset, bool>> predicate, CancellationToken cancellationToken);
}
