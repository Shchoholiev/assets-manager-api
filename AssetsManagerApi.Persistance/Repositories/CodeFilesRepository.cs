using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;

namespace AssetsManagerApi.Persistance.Repositories;
public class CodeFilesRepository(CosmosDbContext db)
    : BaseRepository<CodeFile>(db, "CodeFiles"), ICodeFilesRepository
{
}
