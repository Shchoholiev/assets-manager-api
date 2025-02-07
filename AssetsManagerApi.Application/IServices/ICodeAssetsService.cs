using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Application.Paging;
using AssetsManagerApi.Domain.Entities;
using System.Linq.Expressions;

namespace AssetsManagerApi.Application.IServices;

public interface ICodeAssetsService
{
    Task<PagedList<CodeAssetDto>> GetCodeAssetsPageAsync(CodeAssetFilterModel filterModel, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<CodeAssetDto> GetCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken);

    Task<CodeAssetDto> DeleteCodeAssetAsync(string codeAssetId, CancellationToken cancellationToken);

    Task<CodeAssetDto> CreateCodeAssetAsync(CodeAssetCreateDto createDto, CancellationToken cancellationToken);

    Task<CodeAssetDto> UpdateCodeAssetAsync(CodeAssetUpdateDto dto, CancellationToken cancellationToken);
}
