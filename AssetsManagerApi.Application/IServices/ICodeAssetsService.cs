using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using System.Linq.Expressions;

namespace AssetsManagerApi.Application.IServices;

public interface ICodeAssetsService
{
    Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetDto>> GetUsersCodeAssetsPage(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetDto>> GetCodeAssetsByTagsPage(List<string> tagIds, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<CodeAssetDto>> SearchCodeAssetsPage(string input, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<CodeAssetDto> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken);
}
