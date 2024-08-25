using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;

public class RefreshTokensRepository(CosmosDbContext db) 
    : BaseRepository<RefreshToken>(db, "RefreshTokens"), IRefreshTokensRepository
{
}
