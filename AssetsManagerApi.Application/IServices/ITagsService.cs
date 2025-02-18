using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Application.Paging;

namespace AssetsManagerApi.Application.IServices;
public interface ITagsService
{
    Task<PagedList<TagDto>> GetTagsPage(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<TagDto>> GetPopularTagsPage(string? searchString, int pageNumber, int pageSize, CancellationToken cancellationToken);
}
