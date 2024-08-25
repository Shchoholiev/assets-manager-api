using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;

public class RolesRepository(CosmosDbContext db) 
    : BaseRepository<Role>(db, "Roles"), IRolesRepository
{
}