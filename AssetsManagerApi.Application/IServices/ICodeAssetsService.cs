using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using System.Linq.Expressions;

namespace AssetsManagerApi.Application.IServices;

public interface ICodeAssetsService
{
    Task<PagedList<CodeAssetDto>> GetCodeAssetsPage(CodeAssetFilterModel filterModel, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<CodeAssetDto> GetCodeAssetById(string codeAssetId, CancellationToken cancellationToken);
}
