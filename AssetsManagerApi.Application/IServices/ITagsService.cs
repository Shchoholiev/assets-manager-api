using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Paging;

namespace AssetsManagerApi.Application.IServices;
public interface ITagsService
{
    Task<PagedList<TagDto>> GetTagsPage(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PagedList<TagDto>> GetPopularTagsPage(int pageNumber, int pageSize, CancellationToken cancellationToken);
}
