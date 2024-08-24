using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;

public class UsersRepository(CosmosDbContext db) 
    : BaseRepository<User>(db, "Users"), IUsersRepository
{
}