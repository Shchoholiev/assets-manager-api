using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using System.Linq.Expressions;

namespace AssetsManagerApi.Application.IServices;

public interface ICodeAssetsService
{
    Task<PagedList<CodeAssetResult>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetResult>> GetUsersCodeAssetsPage(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetResult>> GetCodeAssetsByTagsPage(List<string> tagIds, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetResult>> SearchCodeAssetsPage(string input, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<CodeAssetResult> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken);
}
