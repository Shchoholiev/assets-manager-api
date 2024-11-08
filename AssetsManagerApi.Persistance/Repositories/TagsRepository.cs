using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;
using Microsoft.Azure.Cosmos.Linq;

namespace AssetsManagerApi.Persistance.Repositories;

public class TagsRepository(CosmosDbContext db)
    : BaseRepository<Tag>(db, "Tags"), ITagsRepository
{
    public async Task<List<Tag>> GetTagsOrderedByUsageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _container.GetItemLinqQueryable<Tag>()
            .OrderByDescending(tag => tag.UseCount)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToFeedIterator();

        var response = await query.ReadNextAsync(cancellationToken);
        return response.ToList();
    }
}

