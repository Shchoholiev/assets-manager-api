using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;

public class CompaniesRepository(CosmosDbContext db)
    : BaseRepository<Company>(db, "Companies"), ICompaniesRepository
{
}

