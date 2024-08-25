using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;

public class UsersRepository(CosmosDbContext db)
    : BaseRepository<User>(db, "Users"), IUsersRepository
{
    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var partitionKey = new Microsoft.Azure.Cosmos.PartitionKey(user.Id);
        var response = await _container.ReplaceItemAsync(user, user.Id, partitionKey, cancellationToken: cancellationToken);
        return response.Resource;
    }
}