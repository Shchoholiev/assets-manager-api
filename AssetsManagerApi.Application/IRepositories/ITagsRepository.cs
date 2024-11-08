using AssetsManagerApi.Domain.Entities;

namespace AssetsManagerApi.Application.IRepositories;

public interface ITagsRepository : IBaseRepository<Tag>
{
    Task<List<Tag>> GetTagsOrderedByUsageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}
