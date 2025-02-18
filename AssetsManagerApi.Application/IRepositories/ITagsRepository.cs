using AssetsManagerApi.Domain.Entities;
using System.Linq.Expressions;

namespace AssetsManagerApi.Application.IRepositories;

public interface ITagsRepository : IBaseRepository<Tag>
{
    Task<List<Tag>> GetTagsOrderedByUsageAsync(Expression<Func<Tag, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken);
}
