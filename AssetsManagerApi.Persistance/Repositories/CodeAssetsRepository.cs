using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;

public class CodeAssetsRepository(CosmosDbContext db)
    : BaseRepository<CodeAsset>(db, "CodeAssets"), ICodeAssetsRepository
{
}
