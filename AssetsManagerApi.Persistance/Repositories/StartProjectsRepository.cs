using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;
public class StartProjectsRepository(CosmosDbContext db)
    : BaseRepository<StartProject>(db, "StartProjects"), IStartProjectsRepository
{
}